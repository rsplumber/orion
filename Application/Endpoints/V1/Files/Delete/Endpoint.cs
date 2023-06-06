using Core.Files.Services;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Files.Delete;

file sealed class Endpoint : Endpoint<Request>
{
    private readonly IFileService _fileService;

    public Endpoint(IFileService fileService)
    {
        _fileService = fileService;
    }

    public override void Configure()
    {
        Delete("files/{Link}");
        Permissions("orion_put_file");
        Version(1);
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        await _fileService.DeleteAsync(new DeleteFileRequest()
        {
            Link = request.Link
        }, ct);
        await SendOkAsync(ct);
    }
}

file sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Delete file in the system";
        Description = "Delete file in the system";
        Response<PutFileResponse>(200, "File was successfully Deleted");
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