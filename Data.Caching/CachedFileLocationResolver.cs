using System.Text.Json;
using Core.Files;
using Data.Sql.Files;
using Microsoft.Extensions.Caching.Distributed;

namespace Data.Caching;

internal sealed class CachedFileLocationResolver : IFileLocationResolver
{
    private static readonly DistributedCacheEntryOptions DefaultOptions = new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromDays(5));

    private readonly IDistributedCache _cacheService;
    private readonly FileLocationResolver _fileLocationResolver;

    public CachedFileLocationResolver(IDistributedCache cacheService, FileLocationResolver fileLocationResolver)
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
        await _cacheService.SetAsync(link, locationBytes, DefaultOptions, cancellationToken);
        return locations;
    }
}