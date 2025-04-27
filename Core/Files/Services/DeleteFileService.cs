using Core.Providers;
using Core.Providers.Events;
using Core.Providers.Exceptions;
using DotNetCore.CAP;

namespace Core.Files.Services;

internal sealed class DeleteFileService : IDeleteFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly IStorageServiceLocator _storageServiceLocator;
    private readonly ICapPublisher _capPublisher;

    public DeleteFileService(
        IFileRepository fileRepository,
        IProviderRepository providerRepository,
        ICapPublisher capPublisher,
        IStorageServiceLocator storageServiceLocator)
    {
        _fileRepository = fileRepository;
        _providerRepository = providerRepository;
        _capPublisher = capPublisher;
        _storageServiceLocator = storageServiceLocator;
    }

    public async Task DeleteAsync(string link, CancellationToken cancellationToken = default)
    {
        var fileId = IdLink.Parse(link);

        var file = await _fileRepository.FindAsync(fileId, cancellationToken).ConfigureAwait(false);
        if (file is null)
            throw new FileNotFoundException();

        var storageService = await _storageServiceLocator.LocatePrimaryAsync(cancellationToken).ConfigureAwait(false);
        if (storageService is null)
            throw new ProviderNotFoundException();

        await storageService.DeleteAsync(file.Path, file.Name).ConfigureAwait(false);

        RemoveLocation(file.Locations, storageService.Name);

        if (file.Locations.Count == 0)
        {
            await _fileRepository.DeleteAsync(file, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await _fileRepository.UpdateAsync(file, cancellationToken).ConfigureAwait(false);
        }

        var providers = await _providerRepository.FindAsync(cancellationToken).ConfigureAwait(false);

        foreach (var provider in providers)
        {
            _ = _capPublisher.PublishAsync(
                $"{DeleteFileEvent.EventName}.{provider.Name}",
                new DeleteFileEvent
                {
                    FileId = file.Id,
                    Provider = provider.Name
                },
                cancellationToken: cancellationToken);
        }
    }

    private static void RemoveLocation(ICollection<FileLocation> locations, string providerName)
    {
        foreach (var location in locations)
        {
            if (location.Provider == providerName)
            {
                locations.Remove(location);
                break;
            }
        }
    }
}