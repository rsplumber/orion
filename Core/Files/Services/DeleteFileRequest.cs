namespace Core.Files.Services;

public sealed record DeleteFileRequest
{
    public Guid Id { get; set; }
}