﻿using Core.Files.Events;
using Core.Providers.Exceptions;
using DotNetCore.CAP;

namespace Core.Files.Services;

internal sealed class FileLocationService : IFileLocationService
{
    private readonly IFileLocationResolver _fileLocationResolver;
    private readonly ILocationSelector _locationSelector;
    private readonly IFileRepository _fileRepository;
    private readonly IStorageServiceLocator _storageServiceLocator;
    private readonly ICapPublisher _capPublisher;

    public FileLocationService(IFileLocationResolver fileLocationResolver, IFileRepository fileRepository, ILocationSelector locationSelector, ICapPublisher capPublisher, IStorageServiceLocator storageServiceLocator)
    {
        _fileLocationResolver = fileLocationResolver;
        _fileRepository = fileRepository;
        _locationSelector = locationSelector;
        _capPublisher = capPublisher;
        _storageServiceLocator = storageServiceLocator;
    }

    public async Task<string?> GetAsync(string link, CancellationToken cancellationToken = default)
    {
        var fileLocations = await _fileLocationResolver.ResolveAsync(link, cancellationToken);
        if (fileLocations.Count == 0) return null;

        var selectedLocation = await _locationSelector.SelectAsync(fileLocations, cancellationToken);
        if (selectedLocation is null) return null;
        if (IsValidCachedLink())
        {
            return selectedLocation.Link;
        }

        var file = await _fileRepository.FindAsync(IdLink.Parse(link), cancellationToken);
        if (file is null) return null;
        var storageService = await _storageServiceLocator.LocateAsync(selectedLocation.Provider, cancellationToken);
        if (storageService is null) throw new ProviderNotFoundException();
        var refreshedLink = await storageService.RefreshLinkAsync(file.Path, file.Name);
        file.Locations.Clear();
        file.Locations.Add(new FileLocation
        {
            Provider = selectedLocation.Provider,
            Link = refreshedLink.Url,
            ExpireDateUtc = refreshedLink.ExpireDateTimeUtc
        });
        await _fileRepository.UpdateAsync(file, cancellationToken);
        await _capPublisher.PublishAsync(FileLocationRefreshedEvent.EventName, new FileLocationRefreshedEvent
        {
            Provider = file.Locations.First().Provider,
            Id = file.Id
        }, cancellationToken: cancellationToken);
        return refreshedLink.Url;

        bool IsValidCachedLink() => selectedLocation.ExpireDateUtc is not null &&
                                    selectedLocation.ExpireDateUtc > DateTime.UtcNow;
    }
}