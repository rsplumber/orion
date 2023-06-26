using DotNetCore.CAP;

namespace Core;

public sealed class EventsRetriesFailedHandler
{
    private readonly ICapPublisher _capPublisher;

    public EventsRetriesFailedHandler(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    public async Task HandleAsync(Guid identificationStatusId, CancellationToken cancellationToken = default)
    {
    }
}