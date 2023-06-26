using DotNetCore.CAP;

namespace Core.Providers.Events;

public sealed class DeleteFileEvent
{
    public const string EventName = "orion.file.delete";

    public Guid FileId { get; init; }

    public string Provider { get; init; } = default!;
}

internal sealed class DeleteFileEventHandler : ICapSubscribe
{
    private readonly ReplicationService _replicationService;

    public DeleteFileEventHandler(ReplicationService replicationService)
    {
        _replicationService = replicationService;
    }


    [CapSubscribe("orion.file.delete.*", Group = "orion.files.replications.delete.queue")]
    public async Task HandleAsync(DeleteFileEvent message)
    {
        await _replicationService.DeleteReplicationsAsync(new DeleteFileReplicationRequest
        {
            FileId = message.FileId,
            Provider = message.Provider
        });
    }
}