using DotNetCore.CAP;

namespace Core.Replications.Events;

public sealed class DeleteFileEvent
{
    public const string EventName = "orion.file.delete";

    public Guid RequestId { get; set; } = Guid.NewGuid();

    public Guid FileId { get; init; }

    public string Provider { get; init; } = default!;
}

internal sealed class DeleteFileEventHandler : ICapSubscribe
{
    private readonly IEnumerable<AbstractReplicationManagement> _replicationManagements;


    public DeleteFileEventHandler(IEnumerable<AbstractReplicationManagement> replicationManagements)
    {
        _replicationManagements = replicationManagements;
    }

    [CapSubscribe("orion.file.delete.*", Group = "orion.file.delete.queue")]
    public async Task HandleAsync(DeleteFileEvent message)
    {
        var provider = _replicationManagements.First(management => management.Provider == message.Provider);
        await provider.DeleteAsync(new DeleteFileRequest(message.RequestId)
        {
            FileId = message.FileId,
            Provider = message.Provider
        });
    }
}