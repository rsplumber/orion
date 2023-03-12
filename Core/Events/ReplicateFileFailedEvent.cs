﻿using Core.Replications;
using DotNetCore.CAP;

namespace Core.Events;

public sealed record ReplicateFileFailedEvent
{
    public const string EventName = "orion_replication_failed";

    public Guid Id { get; init; }
}

internal sealed class ReplicateFileFailedEventHandler : ICapSubscribe
{
    private readonly IReplicationRepository _replicationRepository;


    public ReplicateFileFailedEventHandler(IReplicationRepository replicationRepository)
    {
        _replicationRepository = replicationRepository;
    }

    [CapSubscribe(ReplicateFileFailedEvent.EventName)]
    public async Task HandleAsync(ReplicateFileFailedEvent message, CancellationToken cancellationToken = default)
    {
        var replication = await _replicationRepository.FindAsync(message.Id, cancellationToken);
        if (replication is null)
        {
            return;
        }

        replication.Failed();
        await _replicationRepository.UpdateAsync(replication, cancellationToken);
    }
}