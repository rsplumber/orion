using Core.Files;
using FastEndpoints;
using FluentValidation;

namespace Application.Endpoints.V1.Files.Put;

file sealed class Endpoint : Endpoint<Request, PutFileResponse>
{
    private readonly IPutFileService _putFileService;


    public Endpoint(IPutFileService putFileService)
    {
        _putFileService = putFileService;
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
        if (Files.Count == 0)
        {
            AddError("File cannot be null or empty");
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        var response = await _putFileService.PutAsync(request.File.OpenReadStream(), new PutFileRequest
        {
            BucketId = request.BucketId,
            Name = request.File.FileName,
            Path = request.FilePath,
            Configs = request.Configs
        }, ct);
        await SendOkAsync(response, ct);
    }
}

file sealed class EndpointSummary : Summary<Endpoint>
{
    public EndpointSummary()
    {
        Summary = "Put file in the system";
        Description = "Put file in the system";
        Response<PutFileResponse>(200, "File was successfully put");
    }
}

file sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(request => request.File)
            .NotEmpty().WithMessage("Add File")
            .NotNull().WithMessage("Add File");

        RuleFor(request => request.FilePath)
            .NotEmpty().WithMessage("Add FilePath")
            .NotNull().WithMessage("Add FilePath");

        RuleFor(request => request.BucketId)
            .NotEmpty().WithMessage("Enter valid BucketId")
            .NotNull().WithMessage("Enter BucketId");
    }
}

file sealed record Request
{
    /// <summary>
    /// e.g: data\files\images or data/files/images
    /// </summary>
    public string FilePath { get; set; } = default!;

    public Guid BucketId { get; set; } = default!;

    public IFormFile File { get; set; } = default!;

    public Dictionary<string, string> Configs { get; set; } = new();
}