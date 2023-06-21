using System.Text.Json;
using Core.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Data.Sql.Files;

internal sealed class FileLocationResolver : IFileLocationResolver
{
    private static readonly DistributedCacheEntryOptions DefaultOptions = new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromDays(5));

    private readonly OrionDbContext _dbContext;
    private readonly IDistributedCache _cacheService;

    public FileLocationResolver(OrionDbContext dbContext, IDistributedCache cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async ValueTask<List<FileLocation>> ResolveAsync(string link, CancellationToken cancellationToken = default)
    {
        var cachedLocations = await _cacheService.GetAsync(link, cancellationToken);
        if (cachedLocations is not null)
        {
            return JsonSerializer.Deserialize<List<FileLocation>>(cachedLocations)!;
        }

        var file = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == IdLink.Parse(link), cancellationToken: cancellationToken);
        if (file is null) return new List<FileLocation>();
        var locationBytes = JsonSerializer.SerializeToUtf8Bytes(file.Locations);
        await _cacheService.SetAsync(link, locationBytes, DefaultOptions, cancellationToken);
        return file.Locations;
    }
}