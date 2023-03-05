namespace Core.FileLocations;

public class FileLocation
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Location { get; set; } = default!;

    public string Provider { get; set; } = default!;

    public Guid FileId { get; set; }
}