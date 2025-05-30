using System.Text.Json;
using Core.Files;
using Microsoft.Extensions.Caching.Distributed;

namespace Data.Caching.InMemory;

internal sealed class CachedFileLocationResolver : IFileLocationResolver
{
    private static readonly DistributedCacheEntryOptions DistributedCacheEntryOptions = new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromDays(5))
        .SetSlidingExpiration(TimeSpan.FromDays(2));

    private readonly IDistributedCache _cacheService;
    private readonly IFileLocationResolver _fileLocationResolver;

    public CachedFileLocationResolver(IDistributedCache cacheService, IFileLocationResolver fileLocationResolver)
    {
        _cacheService = cacheService;
        _fileLocationResolver = fileLocationResolver;
    }

    public async ValueTask<List<FileLocation>> ResolveAsync(string link, CancellationToken cancellationToken = default)
    {
        var cachedLocations = await _cacheService.GetAsync(link, cancellationToken);
        if (cachedLocations is not null)
        {
            return JsonSerializer.Deserialize<List<FileLocation>>(cachedLocations)!;
        }

        var locations = await _fileLocationResolver.ResolveAsync(link, cancellationToken);
        var locationBytes = JsonSerializer.SerializeToUtf8Bytes(locations);
        await _cacheService.SetAsync(link, locationBytes, DistributedCacheEntryOptions, cancellationToken);
        return locations;
    }
}