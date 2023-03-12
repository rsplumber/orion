using Core.Files.Services;
using Data.Sql;
using MinIO;
using Minio.Test;
using Savorboard.CAP.InMemoryMessageQueue;

namespace Application;

internal static class ServiceCollectionExtension
{
    public static void AddOrionService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddData(configuration);
        services.AddScoped<IFileService, FileService>();

        services.AddMinio(configuration);
        services.AddMinioTest(configuration);


        services.AddCap(x =>
        {
            x.UsePostgreSql(configuration.GetConnectionString("Default")
                            ?? throw new ArgumentNullException("connectionString", "Enter connection string in app settings"));
            x.UseInMemoryMessageQueue();
        });
    }
}