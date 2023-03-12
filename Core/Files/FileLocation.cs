namespace Core.Files;

public class FileLocation
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Link { get; set; } = default!;
    
    public string Path { get; set; } = default!;

    public string Provider { get; set; } = default!;

}