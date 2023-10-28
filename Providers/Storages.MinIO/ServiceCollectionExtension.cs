using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Minio;
using Storages.Abstractions;

namespace Storages.MinIO;

public static class ServiceCollectionExtension
{
    public static void AddMinioStorage(this IServiceCollection services, IConfiguration? configuration = default)
    {
        services.AddSingleton<IMinioClient>(_ => new MinioClient()
            .WithEndpoint("10.121.254.62:9100")
            .WithCredentials("oSeAMoNsIVEndENtLESa", "AURnMAyMUckbaFtHEreveRanTECTiM")
            .WithSSL(false)
            .Build());
        services.TryAddScoped<IStorageService, MinIOStorageService>();
    }
}