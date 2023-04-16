using DotNetCore.CAP;

namespace Core.Replications.Events;

public sealed class ReplicateFileEvent
{
    public const string EventName = "orion.file.replicate";

    public Guid RequestId { get; set; } = Guid.NewGuid();

    public Guid FileId { get; init; }

    public string Provider { get; init; } = default!;
}

internal sealed class ReplicateFileEventHandler : ICapSubscribe
{
    private readonly IEnumerable<AbstractReplicationManagement> _replicationManagements;


    public ReplicateFileEventHandler(IEnumerable<AbstractReplicationManagement> replicationManagements)
    {
        _replicationManagements = replicationManagements;
    }

    [CapSubscribe("orion.file.replicate.*", Group = "orion.file.replicate.queue")]
    public async Task HandleAsync(ReplicateFileEvent message)
    {
        var provider = _replicationManagements.First(management => management.Provider == message.Provider);
        await provider.SaveAsync(new ReplicateFileRequest(message.RequestId)
        {
            FileId = message.FileId,
            Provider = message.Provider
        });
    }
}