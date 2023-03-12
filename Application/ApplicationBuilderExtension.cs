using Core.Providers;
using Core.Providers.Types;
using Core.Replications;
using Data.InMemory.Providers.Exceptions;
using Data.Sql;
using Microsoft.EntityFrameworkCore;

namespace Application;

internal static class ApplicationBuilderExtension
{
    public static void UseObjectStorage(this IApplicationBuilder app, IConfiguration configuration)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
        if (serviceScope == null) return;
        try
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<OrionDbContext>();
            context.Database.Migrate();
        }
        catch (Exception)
        {
            // ignored
        }

        var providerRepository = serviceScope.ServiceProvider.GetRequiredService<IProviderRepository>();
        var replicationManagements = serviceScope.ServiceProvider.GetRequiredService<IEnumerable<AbstractReplicationManagement>>();
        foreach (var management in replicationManagements)
        {
            var provider = providerRepository.FindByNameAsync(management.Provider).Result;
            if (provider is not null)
            {
                throw new ProviderNameExistsException(provider.Name);
            }

            providerRepository.AddAsync(new Provider
            {
                Name = management.Provider,
                Status = ProviderStatus.Enable
            }).Wait();
        }
    }
}