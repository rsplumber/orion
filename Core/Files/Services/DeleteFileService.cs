using Core.Providers;
using Core.Replications.Events;
using Core.Storages;
using DotNetCore.CAP;

namespace Core.Files.Services;

internal sealed class DeleteFileService : IDeleteFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly IStorageService _storageService;
    private readonly ICapPublisher _capPublisher;

    public DeleteFileService(IFileRepository fileRepository, IProviderRepository providerRepository, IStorageService storageService, ICapPublisher capPublisher)
    {
        _fileRepository = fileRepository;
        _providerRepository = providerRepository;
        _storageService = storageService;
        _capPublisher = capPublisher;
    }

    public async Task DeleteAsync(string link, CancellationToken cancellationToken = default)
    {
        var fileId = IdLink.Parse(link);
        var file = await _fileRepository.FindAsync(fileId, cancellationToken);
        await _storageService.DeleteAsync(file!.Path, file.Name);
        file.Locations.Remove(file.Locations.First(location => location.Provider == _storageService.Provider));
        if (file.Locations.Count == 0)
        {
            await _fileRepository.DeleteAsync(file, cancellationToken);
            return;
        }

        await _fileRepository.UpdateAsync(file, cancellationToken);
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