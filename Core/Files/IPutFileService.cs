using Core.Files.Exceptions;

namespace Core.Files;

public interface IPutFileService
{
    Task<PutFileResponse> PutAsync(Stream stream, PutFileRequest req, CancellationToken cancellationToken = default);
}

public sealed record PutFileRequest
{
    public required Guid BucketId { get; init; } = default!;
    
    public required string Name { get; init; } = default!;

    public required string Path { get; init; } = default!;

    public Dictionary<string, string>? Configs { get; init; } = new();

    public bool HasConfig() => Configs is not null && Configs.Count > 0;

    public string Extension => System.IO.Path.HasExtension(Name) ? System.IO.Path.GetExtension(Name) : throw new InvalidFileExtensionException();
}

public record PutFileResponse(Guid Id, string Link);