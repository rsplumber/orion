using Core.Files.Events;
using DotNetCore.CAP;

namespace Core.Files.Services;

internal sealed class FilePathFinderService : IFilePathFinderService
{
    private readonly IFileLocationResolver _fileLocationResolver;
    private readonly ILocationSelector _locationSelector;
    private readonly IFileRepository _fileRepository;
    private readonly IStorageServiceLocator _storageServiceLocator;
    private readonly ICapPublisher _capPublisher;

    public FilePathFinderService(IFileLocationResolver fileLocationResolver, IFileRepository fileRepository, ILocationSelector locationSelector, ICapPublisher capPublisher, IStorageServiceLocator storageServiceLocator)
    {
        _fileLocationResolver = fileLocationResolver;
        _fileRepository = fileRepository;
        _locationSelector = locationSelector;
        _capPublisher = capPublisher;
        _storageServiceLocator = storageServiceLocator;
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
        var storageService = await _storageServiceLocator.LocateAsync(selectedLocation.Provider, cancellationToken);
        var refreshedLink = await storageService.RefreshLinkAsync(file.Path, file.Name);
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