namespace Queries.Files;

public class FileResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public Dictionary<string, string> Metas { get; set; }

    public List<FileLocationResponse> Locations { get; set; }

    public DateTime CreatedDateUtc { get;  set; }
}