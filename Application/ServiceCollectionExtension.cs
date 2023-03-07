using Core.Files.Services;
using Data.Sql;
using MinIO;
using Savorboard.CAP.InMemoryMessageQueue;

namespace Application;

internal static class ServiceCollectionExtension
{
    public static void AddObjectStorageService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddData(configuration);
        services.AddScoped<IFileService, FileService>();

        services.AddMinio(configuration);

        services.AddCap(x =>
        {
            x.UsePostgreSql(configuration.GetConnectionString("Default")
                            ?? throw new ArgumentNullException("connectionString", "Enter connection string in app settings"));
            x.UseInMemoryMessageQueue();
        });
    }
}