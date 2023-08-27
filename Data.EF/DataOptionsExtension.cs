using Core.Files;
using Core.Providers;
using Data.Abstractions;
using Data.Abstractions.Providers;
using Data.EF.Files;
using Data.EF.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.EF;

public static class DataOptionsExtension
{
    public static void UseEntityFramework(this DataOptions dataOptions, Action<DbContextOptionsBuilder> optionsAction)
    {
        dataOptions.Services.AddDbContext<OrionDbContext>(optionsAction);
        dataOptions.Services.AddScoped<IFileRepository, FileRepository>();
        dataOptions.Services.AddScoped<IReplicationRepository, ReplicationRepository>();
        dataOptions.Services.AddScoped<IFileLocationResolver, FileLocationResolver>();

        dataOptions.Services.AddScoped<IProviderRepository, ProviderRepository>();
        dataOptions.Services.AddScoped<IProviderQuery, ProviderQuery>();
        dataOptions.Services.AddScoped<IProvidersQuery, ProvidersQuery>();
    }
}