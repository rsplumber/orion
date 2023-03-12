using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Minio.Test;

public static class ServiceCollectionExtension
{
    public static void AddMinioTest(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<MinioClient>(provider => new MinioClient()
            .WithEndpoint("localhost:9000")
            .WithCredentials("EvZWw7VSbGCm4M9D", "5RQNFh5SI2NVxWiAxVp9VK5tvmWQ7BMN")
            .Build());
        services.AddProvider<ReplicateFileManagement>();
    }
}