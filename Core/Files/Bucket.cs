namespace Core.Files;

public sealed class Bucket
{
    public Bucket(string name)
    {
        Name = name;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public List<File> Files { get; private set; } = default!;

    public DateTime CreatedDateUtc { get; private set; } = DateTime.UtcNow;
}