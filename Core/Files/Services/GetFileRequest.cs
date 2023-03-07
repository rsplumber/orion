namespace Core.Files.Services;

public sealed record GetFileRequest
{
    public string Link { get; set; } = default!;
}