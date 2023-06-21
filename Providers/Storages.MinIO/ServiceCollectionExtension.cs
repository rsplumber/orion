using Core.Replications;
using Core.Storages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Minio;

namespace Storages.MinIO;

public static class ServiceCollectionExtension
{
    public static void AddMinio(this IServiceCollection services, Action<ReplicationOption>? setup = null)
    {
        services.TryAddScoped<MinioClient>(provider => new MinioClient()
            .WithEndpoint("10.121.254.62:9100")
            .WithCredentials("oSeAMoNsIVEndENtLESa", "AURnMAyMUckbaFtHEreveRanTECTiM")
            .Build());
        services.TryAddScoped<IStorageService, StorageService>();
    }
}