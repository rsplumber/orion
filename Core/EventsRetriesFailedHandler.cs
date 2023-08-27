using Core.Providers.Events;
using DotNetCore.CAP;

namespace Core;

public sealed class EventsRetriesFailedHandler
{
    private readonly ICapPublisher _capPublisher;

    public EventsRetriesFailedHandler(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    public async Task HandleAsync(Guid fileId, string providerName, CancellationToken cancellationToken = default)
    {
        await _capPublisher.PublishAsync(ReplicateFileFailedEvent.EventName, new ReplicateFileFailedEvent
        {
            FileId = fileId,
            Provider = providerName
        }, cancellationToken: cancellationToken);
    }
}