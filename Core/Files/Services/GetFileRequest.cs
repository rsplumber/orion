namespace Core.Files.Services;

public sealed record GetFileRequest
{
    public string Name { get; set; } = default!;
}