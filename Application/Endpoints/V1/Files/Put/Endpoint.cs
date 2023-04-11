using Core.Files.Services;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Files.Put;

internal sealed class Endpoint : Endpoint<Request, PutFileResponse>
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
        AllowFileUploads();
        Version(1);
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        if (Files.Count > 0)
        {
            var file = Files[0];

            var response = await _fileService.PutAsync(file.OpenReadStream(), new PutFileRequest
            {
                Name = file.FileName,
                Extension = Path.GetExtension(file.FileName),
                FilePath = request.FilePath
            }, ct);
            await SendOkAsync(response, ct);
            return;
        }

        await SendErrorsAsync(cancellation: ct);
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
    /// <summary>
    /// e.g: data\files\images or data/files/images
    /// </summary>
    public string FilePath { get; set; } = default!;

    public IFormFile File { get; set; }
}