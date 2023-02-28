namespace Core.Files;

public class File
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public Dictionary<string, string> Metas { get; set; } = new();

    public Dictionary<string, string> Locations { get; set; } = new();
}