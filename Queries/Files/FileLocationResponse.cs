namespace Queries.Files;

public class FileLocationResponse
{
    public Guid Id { get; set; }

    public string Location { get; set; } = default!;

    public string Provider { get; set; } = default!;
}