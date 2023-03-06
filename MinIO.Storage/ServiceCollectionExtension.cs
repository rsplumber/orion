﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Providers.Abstractions;

namespace MinIO;

public static class ServiceCollectionExtension
{
    public static void AddMinio(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStorageService, StorageService>();
        services.AddSingleton<MinioClient>();
    }
}