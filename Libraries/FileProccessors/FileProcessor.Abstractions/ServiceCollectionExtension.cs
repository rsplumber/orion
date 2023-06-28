using FileProcessor.Abstractions.Locators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FileProcessor.Abstractions;

public static class ServiceCollectionExtension
{
    public static void AddImageProcessors(this IServiceCollection services, IConfiguration? configuration = default)
    {
        services.TryAddScoped<IFileProcessorServiceLocator, FileProcessorServiceLocator>();
    }
}