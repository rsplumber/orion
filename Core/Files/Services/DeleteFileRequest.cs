namespace Core.Files.Services;

public sealed record DeleteFileRequest
{
    public string Link { get; set; }
}