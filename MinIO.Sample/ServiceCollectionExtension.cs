using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Minio;

namespace MinIO.Sample;

public static class ServiceCollectionExtension
{
    public static void AddMinioSample(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<CloudMinioClient>(provider => new CloudMinioClient(new MinioClient()
            .WithEndpoint("192.168.70.119:9002")
            .WithCredentials("nzV2hc4quH1YlGHn", "qWO9NWQ3OJ6JX64h9vXijibU67Q0cRwK")
            .Build()));
        services.AddProvider<ReplicateFileManagement>();
    }
}