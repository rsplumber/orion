namespace Core.Files;

public class FileLocation
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Location { get; set; } = default!;
    
    public string Filename { get; set; } = default!;

    public string Provider { get; set; } = default!;

}