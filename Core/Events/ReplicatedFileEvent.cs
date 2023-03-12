using Core.Replications;
using DotNetCore.CAP;

namespace Core.Events;

public sealed record ReplicatedFileEvent
{
    public const string EventName = "orion_file_replicated";

    public Guid Id { get; init; }
}

internal sealed class NotificationSentEventHandler : ICapSubscribe
{
    private readonly IReplicationRepository _replicationRepository;


    public NotificationSentEventHandler(IReplicationRepository replicationRepository)
    {
        _replicationRepository = replicationRepository;
    }

    [CapSubscribe(ReplicatedFileEvent.EventName)]
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