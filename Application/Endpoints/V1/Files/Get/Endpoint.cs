using FastEndpoints;
using Queries.Files;

namespace Application.Endpoints.V1.Files.Get;

internal sealed class Endpoint : Endpoint<XRequest>
{
    private readonly IFileQuery _fileQuery;


    public Endpoint(IFileQuery fileQuery)
    {
        _fileQuery = fileQuery;
    }

    public override void Configure()
    {
        Get("file/{id}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(XRequest req, CancellationToken ct)
    {
        var fileResponse = await _fileQuery.GetLinkAsync(new Guid(req.Id), ct);
        await SendRedirectAsync(fileResponse.Link, false, ct);
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

internal sealed record XRequest
{
    public string Id { get; set; }
}