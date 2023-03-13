using Application;
using Core;
using Data.InMemory;
using Data.Sql;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MinIO;
using MinIO.Sample;
using Minio.Test;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 30720000000; //set to max allowed file size of your system
});

builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.ListenAnyIP(5161, _ => { });
    options.ListenAnyIP(5162, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        listenOptions.UseHttps();
    });
});

builder.Services.AddCors();

builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc(settings =>
{
    settings.Title = "ObjectStorage - WebApi";
    settings.DocumentName = "v1";
    settings.Version = "v1";
}, addJWTBearerAuth: false, maxEndpointVersion: 1);

builder.Services.AddData(builder.Configuration);
builder.Services.AddCore(builder.Configuration);

builder.Services.AddMinio(builder.Configuration);
builder.Services.AddMinioTest(builder.Configuration);
builder.Services.AddMinioSample(builder.Configuration);


builder.Services.AddCap(x =>
{
    x.UsePostgreSql(builder.Configuration.GetConnectionString("Default")
                    ?? throw new ArgumentNullException("connectionString", "Enter connection string in app settings"));
    x.UseKafka("192.168.70.119:9092");
});

builder.Services.AddInMemoryData();


var app = builder.Build();


app.UseCors(b => b.AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials());

app.UseFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Versioning.Prefix = "v";
    config.Versioning.PrependToRoute = true;
    config.Endpoints.Filter = ep => ep.EndpointTags?.Contains("hidden") is not true;
});

app.UseObjectStorage(builder.Configuration);


// if (app.Environment.IsDevelopment())
// {
app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());
// }


await app.RunAsync();