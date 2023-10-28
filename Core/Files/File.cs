namespace Core.Files;

public sealed class File
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = default!;

    public string Path { get; set; } = default!;

    public Bucket Bucket { get; set; } = default!;

    public Dictionary<string, string> Metas { get; set; } = new();

    public List<FileLocation> Locations { get; set; } = new();

    public DateTime CreatedDateUtc { get; internal set; } = DateTime.UtcNow;

    public void Add(FileLocation location)
    {
        Locations.Add(location);
    }

    public void Remove(FileLocation location)
    {
        Locations.Remove(location);
    }


    public bool ExistLocation(string provider)
    {
        return Locations.Any(location => location.Provider == provider);
    }


    public FileLocation? GetLocation(string provider)
    {
        return Locations.FirstOrDefault(location => location.Provider == provider);
    }
}