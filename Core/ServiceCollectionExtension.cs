using Core.Files;
using Core.Replications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServiceCollectionExtension
{
    public static void AddCore(this IServiceCollection services, IConfiguration configuration)
    {
    }

    public static void AddProvider<TProvider>(this IServiceCollection services)
        where TProvider : AbstractReplicationManagement
    {
        services.AddScoped<AbstractReplicationManagement, TProvider>();
    }
}