using Core.Files;
using Data.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Caching.InMemory;

public static class CachingOptionsExtension
{
    public static void UseInMemoryCaching(this CachingOptions cachingOptions)
    {
        cachingOptions.Services.AddDistributedMemoryCache();
        cachingOptions.Services.Decorate<IFileLocationResolver, CachedFileLocationResolver>();
        cachingOptions.Services.AddTransient<FileDeletedEventHandler>();
        cachingOptions.Services.AddTransient<FileLocationRefreshedEventHandler>();
    }
}