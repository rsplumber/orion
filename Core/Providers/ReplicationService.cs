using Core.Files;
using Core.Files.Events;
using Core.Providers.Events;
using Core.Providers.Exceptions;
using DotNetCore.CAP;

namespace Core.Providers;

public class ReplicationService
{
    private readonly IFileRepository _fileRepository;
    private readonly IStorageServiceLocator _storageServiceLocator;
    private readonly IReplicationRepository _replicationRepository;
    private readonly ICapPublisher _capPublisher;

    public ReplicationService(IStorageServiceLocator storageServiceLocator, IReplicationRepository replicationRepository, ICapPublisher capPublisher, IFileRepository fileRepository)
    {
        _storageServiceLocator = storageServiceLocator;
        _replicationRepository = replicationRepository;
        _capPublisher = capPublisher;
        _fileRepository = fileRepository;
    }

    public virtual async Task ReplicateAsync(ReplicateFileRequest request, CancellationToken cancellationToken = default)
    {
        var file = await _fileRepository.FindAsync(request.FileId, cancellationToken);
        if (file is null) throw new FileNotFoundException();

        var primaryStorageService = await _storageServiceLocator.LocatePrimaryAsync(cancellationToken);
        if (primaryStorageService is null) return;

        var selectedStorageService = await _storageServiceLocator.LocateAsync(request.Provider, cancellationToken);
        if (selectedStorageService is null) return;

        var memoryStream = new MemoryStream();
        try
        {
            await primaryStorageService.GetAsync(file.Path, file.Name, stream =>
            {
                stream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
            });

            await selectedStorageService.PutAsync(memoryStream, file.Path, file.Name);
        }
        catch
        {
            // ignored
        }
        finally
        {
            await memoryStream.DisposeAsync();
        }

        var replication = new Replication
        {
            Provider = selectedStorageService.Name,
            FileId = file.Id
        };
        await _replicationRepository.AddAsync(replication, cancellationToken);

        await _capPublisher.PublishAsync(FileReplicatedEvent.EventName, new FileReplicatedEvent
        {
            Id = replication.Id
        }, cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteReplicationsAsync(DeleteFileReplicationRequest request, CancellationToken cancellationToken = default)
    {
        var file = await _fileRepository.FindAsync(request.FileId, cancellationToken);
        if (file is null) throw new FileNotFoundException();

        var selectedStorageService = await _storageServiceLocator.LocateAsync(request.Provider, cancellationToken);
        if (selectedStorageService is null) throw new ProviderNotFoundException();
        
        await selectedStorageService.DeleteAsync(file.Path, file.Name);
        await _capPublisher.PublishAsync(FileDeletedEvent.EventName, new FileDeletedEvent
        {
            Id = request.FileId
        }, cancellationToken: cancellationToken);
    }
}

public sealed record ReplicateFileRequest
{
    public Guid FileId { get; init; }

    public string Provider { get; init; } = default!;
}

public sealed record DeleteFileReplicationRequest
{
    public Guid FileId { get; init; }

    public string Provider { get; init; } = default!;
}