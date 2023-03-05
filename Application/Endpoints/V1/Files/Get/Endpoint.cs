using FastEndpoints;
using Queries.Files;

namespace Application.Endpoints.V1.Files.Detail;

internal sealed class Endpoint : Endpoint<Request, FileResponse>
{
    private readonly IFileQuery _fileDetailsQuery;


    public Endpoint(IFileQuery fileDetailsQuery)
    {
        _fileDetailsQuery = fileDetailsQuery;
    }

    public override void Configure()
    {
        Get("file/{id}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _fileDetailsQuery.GetAsync(req.Id, ct);
        await SendOkAsync(response, ct);
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