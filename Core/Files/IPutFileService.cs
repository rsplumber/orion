namespace Core.Files;

public interface IPutFileService
{
    Task<PutFileResponse> PutAsync(Stream stream, PutFileRequest req, CancellationToken cancellationToken = default);
}

public sealed record PutFileRequest
{
    public required string Name { get; init; } = default!;

    public required string Path { get; init; } = default!;

    public required Guid OwnerId { get; init; }

    public Dictionary<string, string>? Configs { get; init; } = new();
}

public record PutFileResponse(Guid Id, string Link);