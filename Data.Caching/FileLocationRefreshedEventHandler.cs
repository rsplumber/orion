using Core.Files;
using Core.Files.Events;
using DotNetCore.CAP;
using Microsoft.Extensions.Caching.Distributed;

namespace Data.Caching;

public class FileLocationRefreshedEventHandler : ICapSubscribe
{
    private readonly IDistributedCache _cacheService;

    public FileLocationRefreshedEventHandler(IDistributedCache cacheService)
    {
        _cacheService = cacheService;
    }

    [CapSubscribe(FileLocationRefreshedEvent.EventName, Group = "orion.core.queue")]
    public async Task HandleAsync(FileLocationRefreshedEvent message, CancellationToken cancellationToken = default)
    {
        var fileLink = IdLink.From(message.Id);
        var deletedFileCache = await _cacheService.GetAsync(fileLink, cancellationToken);
        if (deletedFileCache is null) return;
        await _cacheService.RemoveAsync(fileLink, cancellationToken);
    }
}