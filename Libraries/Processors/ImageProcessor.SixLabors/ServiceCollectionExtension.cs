using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImageProcessor.SixLabors;

public static class ServiceCollectionExtension
{
    public static void AddSixLaborsImageProcessor(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileProcessor, ImageProcessor>();
    }
}