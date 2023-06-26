using Core.Providers;
using Core.Providers.Events;
using DotNetCore.CAP;

namespace Core.Files.Services;

internal sealed class DeleteFileService : IDeleteFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly IStorageServiceLocator _storageServiceLocator;
    private readonly ICapPublisher _capPublisher;

    public DeleteFileService(IFileRepository fileRepository, IProviderRepository providerRepository, ICapPublisher capPublisher, IStorageServiceLocator storageServiceLocator)
    {
        _fileRepository = fileRepository;
        _providerRepository = providerRepository;
        _capPublisher = capPublisher;
        _storageServiceLocator = storageServiceLocator;
    }

    public async Task DeleteAsync(string link, CancellationToken cancellationToken = default)
    {
        var fileId = IdLink.Parse(link);
        var file = await _fileRepository.FindAsync(fileId, cancellationToken);
        if (file is null)
        {
            throw new FileNotFoundException();
        }

        var storageService = await _storageServiceLocator.LocatePrimaryAsync(cancellationToken);
        await storageService.DeleteAsync(file.Path, file.Name);
        file.Locations.Remove(file.Locations.First(location => location.Provider == storageService.Name));
        if (file.Locations.Count == 0)
        {
            await _fileRepository.DeleteAsync(file, cancellationToken);
        }
        else
        {
            await _fileRepository.UpdateAsync(file, cancellationToken);
        }

        foreach (var provider in await _providerRepository.FindAsync(cancellationToken))
        {
            await Task.Run(() =>
            {
                _capPublisher.PublishAsync($"{DeleteFileEvent.EventName}.{provider.Name}", new DeleteFileEvent
                {
                    FileId = file.Id,
                    Provider = provider.Name
                }, cancellationToken: cancellationToken);
            }, cancellationToken);
        }
    }
}