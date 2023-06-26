using Core.Providers;
using Core.Providers.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storages.Abstractions;

namespace Data.Sql;

public static class ApplicationBuilderExtension
{
    public static void UseData(this IApplicationBuilder app, IConfiguration? configuration = default)
    {
        var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
        if (serviceScope == null) return;
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