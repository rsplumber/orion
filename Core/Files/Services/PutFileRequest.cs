namespace Core.Files.Services;

public sealed record PutFileRequest
{
    public string Name { get; init; } = default!;
    
    public string FilePath { get; init; } = default!;

    public string Extension { get; init; } = default!;

    public string ContentType { get; init; } = default!;
}