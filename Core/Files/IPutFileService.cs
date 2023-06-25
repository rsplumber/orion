namespace Core.Files;

public interface IPutFileService
{
    Task<PutFileResponse> PutAsync(Stream stream, PutFileRequest req, CancellationToken cancellationToken = default);
}

public sealed record PutFileRequest
{
    public required string Name { get; init; } = default!;

    public required string Extension { get; init; } = default!;

    public required string FilePath { get; init; } = default!;

    public Dictionary<string, string>? Configs { get; init; } = new();

    public required Guid OwnerId { get; init; }
}

public record PutFileResponse(Guid Id, string Link);