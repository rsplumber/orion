using FileProcessor.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileProcessor.Images.SixLabors;

public static class ServiceCollectionExtension
{
    public static void AddSixLaborsImageProcessor(this IServiceCollection services, IConfiguration? configuration = default)
    {
        services.AddScoped<IFileProcessor, ImageProcessor>();
    }
}