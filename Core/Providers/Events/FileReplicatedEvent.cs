using DotNetCore.CAP;

namespace Core.Providers.Events;

public sealed record FileReplicatedEvent
{
    public const string EventName = "orion.file.replicated";

    public Guid Id { get; init; }
}

internal sealed class FileReplicatedEventHandler : ICapSubscribe
{
    private readonly IReplicationRepository _replicationRepository;


    public FileReplicatedEventHandler(IReplicationRepository replicationRepository)
    {
        _replicationRepository = replicationRepository;
    }

    [CapSubscribe(FileReplicatedEvent.EventName, Group = "orion.files.replications.queue")]
    public async Task HandleAsync(FileReplicatedEvent message, CancellationToken cancellationToken = default)
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