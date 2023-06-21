using Core.Files;
using Core.Files.Events;
using DotNetCore.CAP;
using Microsoft.Extensions.Caching.Distributed;

namespace Data.Caching;

public class FileDeletedEventHandler : ICapSubscribe
{
    private readonly IDistributedCache _cacheService;

    public FileDeletedEventHandler(IDistributedCache cacheService)
    {
        _cacheService = cacheService;
    }

    [CapSubscribe("orion.file.deleted.*", Group = "orion.core.queue")]
    public async Task HandleAsync(FileDeletedEvent message, CancellationToken cancellationToken = default)
    {
        var fileLink = IdLink.From(message.Id);
        var deletedFileCache = await _cacheService.GetAsync(fileLink, cancellationToken);
        if (deletedFileCache is null) return;
        await _cacheService.RemoveAsync(fileLink, cancellationToken);
    }
}