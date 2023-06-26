using Core.Files;
using Core.Providers;
using Data.Sql.Files;
using Data.Sql.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Queries.Providers;

namespace Data.Sql;

public static class ServiceCollectionExtension
{
    public static void AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrionDbContext>(
            builder => builder.UseNpgsql(configuration.GetConnectionString("Default")));
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IReplicationRepository, ReplicationRepository>();
        services.AddScoped<FileLocationResolver>();
        services.AddScoped<IFileLocationResolver, FileLocationResolver>();

        services.AddScoped<IProviderRepository, ProviderRepository>();
        services.AddScoped<IProviderQuery, ProviderQuery>();
        services.AddScoped<IProvidersQuery, ProvidersQuery>();
    }
}