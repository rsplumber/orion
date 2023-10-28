using Core.Files;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Files.Delete;

file sealed class Endpoint : Endpoint<Request>
{
    private readonly IDeleteFileService _deleteFileService;

    public Endpoint(IDeleteFileService deleteFileService)
    {
        _deleteFileService = deleteFileService;
    }

    public override void Configure()
    {
        Delete("files/{Link}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await _deleteFileService.DeleteAsync(req.Link, ct);
        await SendOkAsync(ct);
    }
}

file sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Delete file in the system";
        Description = "Delete file in the system";
        Response(200, "File was successfully Deleted");
    }
}

file sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(request => request.Link)
            .NotEmpty().WithMessage("Add Link")
            .NotNull().WithMessage("Add Link");
    }
}

file sealed record Request
{
    public string Link { get; set; } = default!;
}