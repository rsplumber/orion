namespace Core.Files.Services;

public class PutFileRequest
{
    public string Name { get; set; } = default!;

    public string Extension { get; set; } = default!;

    public string Mime { get; set; } = default!;

    public byte[] Content { get; set; }
}