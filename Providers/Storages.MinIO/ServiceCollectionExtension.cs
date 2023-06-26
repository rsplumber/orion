using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Minio;
using Storages.Abstractions;

namespace Storages.MinIO;

public static class ServiceCollectionExtension
{
    public static void AddMinio(this IServiceCollection services, IConfiguration? configuration = default)
    {
        services.TryAddScoped<MinioClient>(provider => new MinioClient()
            .WithEndpoint("10.121.254.62:9100")
            .WithCredentials("oSeAMoNsIVEndENtLESa", "AURnMAyMUckbaFtHEreveRanTECTiM")
            .Build());
        services.TryAddScoped<IStorageService, MinIOStorageService>();
    }
}