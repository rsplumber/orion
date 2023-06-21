using DotNetCore.CAP;

namespace Core.Replications.Events;

public sealed record ReplicatedFileEvent
{
    public const string EventName = "orion.file.replicated";

    public Guid Id { get; init; }
}

internal sealed class ReplicatedFileEventHandler : ICapSubscribe
{
    private readonly IReplicationRepository _replicationRepository;


    public ReplicatedFileEventHandler(IReplicationRepository replicationRepository)
    {
        _replicationRepository = replicationRepository;
    }

    [CapSubscribe(ReplicatedFileEvent.EventName, Group = "orion.files.replications.queue")]
    public async Task HandleAsync(ReplicatedFileEvent message, CancellationToken cancellationToken = default)
    {
        var replication = await _replicationRepository.FindAsync(message.Id, cancellationToken);
        if (replication is null)
        {
            return;
        }

        replication.Sent();
        await _replicationRepository.UpdateAsync(replication, cancellationToken);
    }
}