using Core.Files.Services;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Files.Put;

internal sealed class Endpoint : EndpointWithoutRequest
{
    private readonly IFileService _fileService;

    public Endpoint(IFileService fileService)
    {
        _fileService = fileService;
    }

    public override void Configure()
    {
        Put("files");
        AllowAnonymous();
        AllowFileUploads(dontAutoBindFormData: true);
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {

        var location = string.Empty;
        await foreach (var section in FormFileSectionsAsync(ct))
        {
            if (section is null) continue;
            location = await _fileService.PutAsync(section.Section.Body, new PutFileRequest
            {
                Name = section.FileName,
                Extension = Path.GetExtension(section.FileName),
            }, ct);
        }

        await SendOkAsync(location, ct);
    }
}

internal sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Put file in the system";
        Description = "Put file in the system";
        Response(200, "File was successfully put");
    }
}

internal sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(request => request.File)
            .NotEmpty().WithMessage("Put or Upload File")
            .NotNull().WithMessage("Put or Upload File");
    }
}

internal sealed record Request
{
    public IFormFile File { get; set; }
}