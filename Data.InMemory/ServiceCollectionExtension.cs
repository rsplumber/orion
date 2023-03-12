using Core.Providers;
using Data.InMemory.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Queries.Providers;

namespace Data.InMemory;

public static class ServiceCollectionExtension
{
    public static void AddInMemoryData(this IServiceCollection services)
    {
        services.TryAddSingleton<IProviderRepository, ProviderRepository>();
        services.AddScoped<IProviderDetailsQuery, ProviderDetailsQuery>();
        services.AddScoped<IProviderListQuery, ProviderListQuery>();
    }
}