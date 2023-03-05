using Core.Files.Services;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Files.Put;

internal sealed class Endpoint : Endpoint<Request>
{
    private readonly IFileService _fileService;

    public Endpoint(IFileService fileService)
    {
        _fileService = fileService;
    }

    public override void Configure()
    {
        Post("file/put");
        AllowAnonymous();
        AllowFileUploads();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var fileExtension = Path.GetExtension(req.File.FileName);
        var fileName = Guid.NewGuid() + "_" + req.File.FileName;
        var filePath = Path.Combine(".\\Files", fileName);


        if (req.File.Length > 0)
        {
            await using (Stream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await req.File.CopyToAsync(fileStream, ct);
            }
        }

        var request = new PutFileRequest
        {
            Name = fileName,
            FilePath = filePath,
            Extension = fileExtension,
            ContentType = req.File.ContentType
        };

        await _fileService.PutAsync(request, ct);
        await SendOkAsync(ct);
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