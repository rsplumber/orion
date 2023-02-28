namespace Core.Files.Services;

public class PutFileRequest
{
    public string FilePath { get; set; } = default!;

    public string Extension { get; set; } = default!;
    public string Mime { get; set; } = default!;
}