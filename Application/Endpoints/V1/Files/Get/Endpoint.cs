using FastEndpoints;
using Providers.Abstractions;
using Queries.Files;

namespace Application.Endpoints.V1.Files.Get;

internal sealed class Endpoint : Endpoint<Request, FileResponse>
{
    private readonly IFileQuery _fileDetailsQuery;
    private readonly IStorageService _storageService;


    public Endpoint(IFileQuery fileDetailsQuery, IStorageService storageService)
    {
        _fileDetailsQuery = fileDetailsQuery;
        _storageService = storageService;
    }

    public override void Configure()
    {
        Get("file/{id}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _storageService.GetAsync(new GetObject()
        {
            Name = "d617fde4-7127-4ecb-a902-8f505b4cea1a_20230130_125210.jpg"
        });
        // var response = await _fileDetailsQuery.GetLinkAsync(req.Id, ct);
        await SendOkAsync(ct);
    }
}

internal sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Get file detail in the system";
        Description = "Get file detail in the system";
        Response(200, "File detail was successfully returned");
    }
}

internal sealed record Request
{
    public Guid Id { get; set; }
}