using System.Text.Json;
using Application;
using Core;
using Data.Caching;
using Data.Sql;
using DotNetCore.CAP.Messages;
using Elastic.Apm.NetCoreAll;
using FastEndpoints;
using FastEndpoints.Swagger;
using ImageProcessor.SixLabors;
using KunderaNet.FastEndpoints.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Storages.MinIO;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options => { options.Limits.MaxRequestBodySize = 50_000_000; });

builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.ListenAnyIP(5161, _ => { });
    // options.ListenAnyIP(5162, listenOptions =>
    // {
    //     listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
    //     listenOptions.UseHttps();
    // });
});
builder.Services.AddHealthChecks();
builder.Services.AddCors();
builder.Services.AddAuthentication(KunderaDefaults.Scheme)
    .AddKundera(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.TryAddSingleton<ExceptionHandlerMiddleware>();

builder.Services.AddFastEndpoints();

builder.Services.SwaggerDocument(settings =>
{
    settings.DocumentSettings = generatorSettings =>
    {
        generatorSettings.Title = "ObjectStorage - WebApi";
        generatorSettings.DocumentName = "v1";
        generatorSettings.Version = "v1";
        generatorSettings.AddKunderaAuth();
    };
    settings.EnableJWTBearerAuth = false;
    settings.MaxEndpointVersion = 1;
});


builder.Services.AddCap(options =>
{
    options.FailedRetryCount = 5;
    options.FailedRetryInterval = 60 * 2;
    options.FailedMessageExpiredAfter = 60 * 60 * 3;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.IgnoreReadOnlyFields = true;

    async void OptionsFailedThresholdCallback(FailedInfo info)
    {
        if (info.Message.Value is null) return;
        // await using var scope = info.ServiceProvider.CreateAsyncScope();
        // var handler = scope.ServiceProvider.GetRequiredService<EventsRetriesFailedHandler>();
        // await handler.HandleAsync(id, CancellationToken.None);
    }

    options.FailedThresholdCallback = OptionsFailedThresholdCallback;
    options.UseRabbitMQ(op =>
    {
        op.HostName = builder.Configuration.GetValue<string>("RabbitMQ:HostName") ?? throw new ArgumentNullException("RabbitMQ:HostName", "Enter RabbitMQ:HostName in app settings");
        op.UserName = builder.Configuration.GetValue<string>("RabbitMQ:UserName") ?? throw new ArgumentNullException("RabbitMQ:UserName", "Enter RabbitMQ:UserName in app settings");
        op.Password = builder.Configuration.GetValue<string>("RabbitMQ:Password") ?? throw new ArgumentNullException("RabbitMQ:Password", "Enter RabbitMQ:UserName in app settings");
        op.ExchangeName = builder.Configuration.GetValue<string>("RabbitMQ:ExchangeName") ?? throw new ArgumentNullException("RabbitMQ:ExchangeName", "Enter RabbitMQ:ExchangeName in app settings");
    });
    options.UsePostgreSql(sqlOptions =>
    {
        sqlOptions.ConnectionString = builder.Configuration.GetConnectionString("default") ?? throw new ArgumentNullException("connectionString", "Enter connection string in app settings");
        sqlOptions.Schema = "events";
    });
});


builder.Services.AddData(builder.Configuration);
builder.Services.AddCaching(builder.Configuration);
builder.Services.AddCore(builder.Configuration);
builder.Services.AddMinio();
builder.Services.AddSixLaborsImageProcessor(builder.Configuration);

var app = builder.Build();

app.UseCors(b => b.AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials());

app.UseHealthChecks("/health");
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseData(builder.Configuration);
app.UseAllElasticApm(builder.Configuration);
app.UseFastEndpoints(config =>
{
    config.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    config.Endpoints.RoutePrefix = "orion/api";
    config.Versioning.Prefix = "v";
    config.Versioning.PrependToRoute = true;
});


// if (app.Environment.IsDevelopment())
// {
app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());
// }


await app.RunAsync();