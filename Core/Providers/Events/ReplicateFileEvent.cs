using DotNetCore.CAP;

namespace Core.Providers.Events;

public sealed class ReplicateFileEvent
{
    public const string EventName = "orion.file.replicate";

    public Guid FileId { get; init; }

    public string Provider { get; init; } = default!;
}

internal sealed class ReplicateFileEventHandler : ICapSubscribe
{
    private readonly ReplicationService _replicationService;

    public ReplicateFileEventHandler(ReplicationService replicationService)
    {
        _replicationService = replicationService;
    }


    [CapSubscribe("orion.file.replicate.*", Group = "orion.files.replications.queue")]
    public async Task HandleAsync(ReplicateFileEvent message)
    {
        await _replicationService.ReplicateAsync(new ReplicateFileRequest
        {
            FileId = message.FileId,
            Provider = message.Provider
        });
    }
}