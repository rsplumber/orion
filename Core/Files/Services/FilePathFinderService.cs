using System.Diagnostics;
using Core.Files.Events;
using Core.Storages;
using DotNetCore.CAP;

namespace Core.Files.Services;

internal sealed class FilePathFinderService : IFilePathFinderService
{
    private readonly IFileLocationResolver _fileLocationResolver;
    private readonly ILocationSelector _locationSelector;
    private readonly IFileRepository _fileRepository;
    private readonly IStorageService _storageService;
    private readonly ICapPublisher _capPublisher;

    public FilePathFinderService(IFileLocationResolver fileLocationResolver, IFileRepository fileRepository, IStorageService storageService, ILocationSelector locationSelector, ICapPublisher capPublisher)
    {
        _fileLocationResolver = fileLocationResolver;
        _fileRepository = fileRepository;
        _storageService = storageService;
        _locationSelector = locationSelector;
        _capPublisher = capPublisher;
    }

    public async Task<string?> GetAbsolutePathAsync(string link, CancellationToken cancellationToken = default)
    {
        var fileLocations = await _fileLocationResolver.ResolveAsync(link, cancellationToken);
        if (fileLocations.Count == 0) return null;

        var selectedLocation = await _locationSelector.SelectAsync(fileLocations, cancellationToken);
        if (selectedLocation is null) return null;
        if (selectedLocation.ExpireDateUtc is not null && selectedLocation.ExpireDateUtc >= DateTime.UtcNow) return selectedLocation.Link;

        var file = await _fileRepository.FindAsync(IdLink.Parse(link), cancellationToken);
        if (file is null) return null;
        var refreshedLink = await _storageService.RefreshLinkAsync(file.Path, file.Name);
        var needToUpdateLocation = file.Locations.First(location => location.Link == selectedLocation.Link);
        needToUpdateLocation.Link = refreshedLink.Url;
        needToUpdateLocation.ExpireDateUtc = refreshedLink.ExpireDateTimeUtc;
        await _fileRepository.UpdateAsync(file, cancellationToken);
        await _capPublisher.PublishAsync(FileLocationRefreshedEvent.EventName, new FileLocationRefreshedEvent
        {
            Provider = needToUpdateLocation.Provider,
            Id = file.Id
        }, cancellationToken: cancellationToken);
        return refreshedLink.Url;
    }
}