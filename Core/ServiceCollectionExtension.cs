using Core.Files;
using Core.Files.Services;
using Core.Locators;
using Core.Providers;
using Core.Providers.Events;
using Core.Providers.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServiceCollectionExtension
{
    public static void AddCore(this IServiceCollection services, IConfiguration? configuration = default)
    {
        services.AddScoped<IFilePathFinderService, FilePathFinderService>();
        services.AddScoped<IPutFileService, PutFileService>();
        services.AddScoped<IDeleteFileService, DeleteFileService>();
        services.AddScoped<ILocationSelector, LocationSelector>();

        services.AddTransient<ReplicateFileEventHandler>();
        services.AddTransient<ReplicateFileFailedEventHandler>();
        services.AddTransient<FileReplicatedEventHandler>();
        services.AddTransient<EventsRetriesFailedHandler>();
        services.AddTransient<ReplicationService>();

        services.AddScoped<IStorageServiceLocator, StorageServiceLocator>();
        services.AddScoped<IProviderService, ProviderService>();
    }
}