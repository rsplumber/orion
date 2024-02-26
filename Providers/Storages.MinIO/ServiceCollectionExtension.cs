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
            .WithEndpoint("app.sbank.ir")
            .WithCredentials("oSeAMoNsIVEndENtLESa", "AURnMAyMUckbaFtHEreveRanTECTiM")
            .WithSSL()
            .Build());

        services.AddSingleton<CustomMinIoClient>(_ => (CustomMinIoClient) new CustomMinIoClient()
            .WithEndpoint("10.255.255.76:9100")
            .WithCredentials("oSeAMoNsIVEndENtLESa", "AURnMAyMUckbaFtHEreveRanTECTiM")
            .WithSSL(false)
            .Build());
        services.TryAddScoped<IStorageService, MinIOStorageService>();
    }
}