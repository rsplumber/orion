using Core.Files.Services;
using Data.Sql;
using MinIO;
using Savorboard.CAP.InMemoryMessageQueue;

namespace Application;

internal static class ServiceCollectionExtension
{
    public static void AddObjectStorageService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCap(x =>
        {
            x.UsePostgreSql(configuration.GetConnectionString("default")
                            ?? throw new ArgumentNullException("connectionString", "Enter connection string in app settings"));
            x.UseKafka("192.168.70.119:9092");
        });
        
        services.AddData(configuration);
        services.AddScoped<IFileService, FileService>();

        services.AddCap(x =>
        {
            x.UseInMemoryMessageQueue();
            x.UseInMemoryStorage();
            x.UseDashboard();
        });

        services.AddMinio(configuration);
    }
}