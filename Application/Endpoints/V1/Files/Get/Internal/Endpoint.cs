using Core.Files;
using DotNetCore.CAP;
using FastEndpoints;
using FluentValidation;
using Minio.DataModel.Args;
using Storages.MinIO;

namespace Application.Endpoints.V1.Files.Get.Internal;

file sealed class Endpoint : Endpoint<Request>
{
    private readonly IFileLocationResolver _fileLocationResolver;
    private readonly ILocationSelector _locationSelector;
    private readonly IFileRepository _fileRepository;
    private const int LinkExpireTimeInSeconds = 518400;
    private readonly CustomMinIoClient _customMinIoClient;

    public Endpoint(IFileLocationResolver fileLocationResolver, ILocationSelector locationSelector, IFileRepository fileRepository, ICapPublisher capPublisher, CustomMinIoClient customMinIoClient)
    {
        _fileLocationResolver = fileLocationResolver;
        _locationSelector = locationSelector;
        _fileRepository = fileRepository;
        _customMinIoClient = customMinIoClient;
    }

    public override void Configure()
    {
        Get("internal/files/{link}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var file = await _fileRepository.FindAsync(IdLink.Parse(req.Link), ct);
        if (file is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var (bucketName, filePath) = ExtractPathData(file.Path);
        var objectName = $"{filePath}/{file.Name}";
        var url = await _customMinIoClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry(LinkExpireTimeInSeconds))
            .ConfigureAwait(false);
        await SendRedirectAsync(url, false, ct);
    }

    private static (string, string) ExtractPathData(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return (string.Empty, string.Empty);
        }

        var pathArray = SplitPath(filePath);
        if (pathArray.Length == 0) return (string.Empty, string.Empty);
        var bucketName = pathArray.First();
        var finalPath = string.Join("/", pathArray[1..]);
        return (bucketName, finalPath);

        string[] SplitPath(string path)
        {
            var splitPath = path.Split("/");
            var finalSplit = splitPath.Length > 1 ? splitPath : path.Split("\\");
            return finalSplit.Where(s => s.Length > 0).ToArray();
        }
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