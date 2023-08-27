using DotNetCore.CAP;

namespace Core.Providers.Events;

public sealed record ReplicateFileFailedEvent
{
    public const string EventName = "orion.replication.failed";

    public Guid FileId { get; init; }

    public string Provider { get; init; } = default!;
}

internal sealed class ReplicateFileFailedEventHandler : ICapSubscribe
{
    private readonly IReplicationRepository _replicationRepository;


    public ReplicateFileFailedEventHandler(IReplicationRepository replicationRepository)
    {
        _replicationRepository = replicationRepository;
    }

    [CapSubscribe(ReplicateFileFailedEvent.EventName, Group = "orion.files.replications.queue")]
    public async Task HandleAsync(ReplicateFileFailedEvent message, CancellationToken cancellationToken = default)
    {
        var replication = await _replicationRepository.FindAsync(message.FileId, cancellationToken);
        if (replication is null)
        {
            return;
        }

        replication.Failed();
        await _replicationRepository.UpdateAsync(replication, cancellationToken);
    }
}