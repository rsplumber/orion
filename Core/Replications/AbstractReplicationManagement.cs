using Core.Replications.Events;
using DotNetCore.CAP;

namespace Core.Replications;

public abstract class AbstractReplicationManagement
{
    private readonly ICapPublisher _eventBus;
    private readonly IReplicationRepository _replicationRepository;


    protected AbstractReplicationManagement(ICapPublisher eventBus, IReplicationRepository replicationRepository)
    {
        _eventBus = eventBus;
        _replicationRepository = replicationRepository;
    }

    public abstract string Provider { get; }

    protected abstract int MaximumRetryCount { get; }

    protected abstract Task<bool> ReplicateFileAsync(Guid filedId, CancellationToken cancellationToken);

    protected virtual void Validate(Guid filedId)
    {
    }

    public async Task SaveAsync(ReplicateFileRequest req, CancellationToken cancellationToken = default)
    {
        var replication = await GetOrAddReplication(req, cancellationToken);

        if (MaximumRetryReached(replication))
        {
            await RaiseFailedEventAsync(replication, cancellationToken);
            return;
        }

        bool replicationSent;

        Validate(req.FileId);
        replicationSent = await ReplicateFileAsync(req.FileId, cancellationToken);


        if (!replicationSent)
        {
            await RaiseReplicateEventAsync(req, cancellationToken);
            return;
        }

        await RaiseReplicatedEventAsync(req.Id, cancellationToken);
    }


    private bool MaximumRetryReached(Replication replication)
    {
        return replication.Retry >= MaximumRetryCount;
    }

    private async Task<Replication> GetOrAddReplication(ReplicateFileRequest req, CancellationToken cancellationToken)
    {
        var replication = await _replicationRepository.FindAsync(req.Id, cancellationToken);
        if (replication is null)
        {
            var createdNotification = new Replication
            {
                Id = req.Id,
                Provider = Provider,
                FileId = req.FileId,
            };
            await _replicationRepository.AddAsync(createdNotification, cancellationToken);
            return createdNotification;
        }

        replication.IncrementRetry();
        await _replicationRepository.UpdateAsync(replication, cancellationToken);
        return replication;
    }

    private async Task RaiseReplicateEventAsync(ReplicateFileRequest req, CancellationToken cancellationToken = default)
    {
        await _eventBus.PublishAsync(ReplicateFileEvent.EventName, new ReplicateFileEvent
        {
            RequestId = req.Id,
            FileId = req.FileId,
            Provider = req.Provider
        }, cancellationToken: cancellationToken);
    }

    private async Task RaiseFailedEventAsync(Replication replication, CancellationToken cancellationToken = default)
    {
        await _eventBus.PublishAsync(ReplicateFileFailedEvent.EventName, new ReplicatedFileEvent()
        {
            Id = replication.Id
        }, cancellationToken: cancellationToken);
    }

    private async Task RaiseReplicatedEventAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _eventBus.PublishAsync(ReplicatedFileEvent.EventName, new ReplicatedFileEvent
        {
            Id = id
        }, cancellationToken: cancellationToken);
    }
}