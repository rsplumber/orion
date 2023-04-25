namespace Core.Files.Services;

public sealed record PutFileRequest
{
    public string Name { get; init; } = default!;

    public string Extension { get; init; } = default!;

    public long Lenght { get; init; } = default!;
    
    public string FilePath { get; init; } = default!;
    
    public Guid OwnerId { get; init; } = default!;
}