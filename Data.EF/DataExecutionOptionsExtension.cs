using Core.Providers;
using Core.Providers.Types;
using Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Storages.Abstractions;

namespace Data.EF;

public static class DataExecutionOptionsExtension
{
    public static void UseEntityFramework(this DataExecutionOptions dataExecutionOptions)
    {
        using var serviceScope = dataExecutionOptions.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        try
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<OrionDbContext>();
            context.Database.Migrate();
            var storages = serviceScope.ServiceProvider.GetRequiredService<IEnumerable<IStorageService>>();
            var providers = context.Providers.ToList();
            context.Providers.RemoveRange(providers.Where(provider => storages
                .All(storage => storage.Name != provider.Name)));
            context.SaveChanges();

            var storageServices = storages as IStorageService[] ?? storages.ToArray();
            var providerService = serviceScope.ServiceProvider.GetRequiredService<IProviderService>();

            foreach (var storage in storageServices)
            {
                if (providers.Any(provider => provider.Name == storage.Name)) continue;
                providerService.AddAsync(new Provider
                {
                    Name = storage.Name,
                    Status = ProviderStatus.Enable
                });
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}