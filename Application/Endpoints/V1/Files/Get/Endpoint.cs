using Core.Files.Services;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Files.Get;

file sealed class Endpoint : Endpoint<Request>
{
    private readonly IFileService _fileService;

    public Endpoint(IFileService fileService)
    {
        _fileService = fileService;
    }

    public override void Configure()
    {
        Get("files/{link}");
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

file sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Get file location in the system";
        Description = "Get file location in the system";
        Response(301, "Redirect to file location");
    }
}

file sealed record Request
{
    public string Link { get; set; } = default!;
}

file sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(request => request.Link)
            .NotEmpty().WithMessage("Enter Link")
            .NotNull().WithMessage("Enter Link");
    }
}