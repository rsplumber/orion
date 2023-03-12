using Core.Files;
using Core.Replications;
using Data.Sql.Files;
using Data.Sql.Replications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Queries.Files;

namespace Data.Sql;

public static class ServiceCollectionExtension
{
    public static void AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrionDbContext>(
            builder => builder.UseNpgsql(configuration.GetConnectionString("Default")));
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IReplicationRepository, ReplicationRepository>();
        services.AddScoped<IFileQuery, FileQuery>();
    }
}