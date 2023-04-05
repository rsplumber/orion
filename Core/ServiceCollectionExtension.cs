using Core.Files.Services;
using Core.Replications;
using Core.Replications.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServiceCollectionExtension
{
    public static void AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ReplicateFileEventHandler>();
        services.AddScoped<ReplicateFileFailedEventHandler>();
        services.AddScoped<ReplicatedFileEventHandler>();
    }

    public static void AddProvider<TProvider>(this IServiceCollection services)
        where TProvider : AbstractReplicationManagement
    {
        services.AddScoped<AbstractReplicationManagement, TProvider>();
    }
}