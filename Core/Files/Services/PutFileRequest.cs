namespace Core.Files.Services;

public class PutFileRequest
{
    public string Name { get; set; } = default!;
    
    public string FilePath { get; set; } = default!;

    public string Extension { get; set; } = default!;

    public string ContentType { get; set; } = default!;
}