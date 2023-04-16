﻿using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Minio;

namespace MinIO;

public static class ServiceCollectionExtension
{
    public static void AddMinio(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<MinioClient>(provider => new MinioClient()
            .WithEndpoint("192.168.70.117:9000")
            .WithCredentials("EvZWw7VSbGCm4M9D", "5RQNFh5SI2NVxWiAxVp9VK5tvmWQ7BMN")
            .Build());
        services.TryAddScoped<IStorageService, StorageService>();
    }
}