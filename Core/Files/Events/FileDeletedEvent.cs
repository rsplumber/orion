using Core.Providers;
using DotNetCore.CAP;

namespace Core.Files.Events;

public sealed class FileDeletedEvent
{
    public const string EventName = "orion.file.deleted";

    public Guid Id { get; set; } = Guid.NewGuid();
}

internal sealed class FileDeletedEventHandler : ICapSubscribe
{
    private readonly IReplicationRepository _replicationRepository;
    private readonly IFileRepository _fileRepository;

    public FileDeletedEventHandler(IReplicationRepository replicationRepository, IFileRepository fileRepository)
    {
        _replicationRepository = replicationRepository;
        _fileRepository = fileRepository;
    }

    [CapSubscribe("orion.file.deleted.*", Group = "orion.core.queue")]
    public async Task HandleAsync(FileDeletedEvent message)
    {
        var replication = await _replicationRepository.FindAsync(message.Id);
        if (replication is null) return;
        var file = await _fileRepository.FindAsync(replication.FileId);
        if (file is null) return;
        file.Locations.Remove(file.Locations.First(location => location.Provider == replication.Provider));
        await _replicationRepository.DeleteAsync(replication);
        if (file.Locations.Count == 0)
        {
            await _fileRepository.DeleteAsync(file);
            return;
        }

        await _fileRepository.UpdateAsync(file);
    }
}