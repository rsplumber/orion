using Core.Files.Services;
using FastEndpoints;

namespace Application.Endpoints.V1.Files.Get;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly IFileService _fileService;

    public Endpoint(IFileService fileService)
    {
        _fileService = fileService;
    }

    public override void Configure()
    {
        Get("files/{Link}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var location = await _fileService.GetLocationAsync(new GetFileRequest
        {
            Link = req.Link
        }, ct);
        await SendRedirectAsync(location, false, ct);
    }
}

internal sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Get file location in the system";
        Description = "Get file location in the system";
        Response(200, "File location was successfully returned");
    }
}

internal sealed record Request
{
    public string Link { get; set; }
}