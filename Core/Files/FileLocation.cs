namespace Core.Files;

public sealed record FileLocation
{
    public string Link { get; set; } = default!;

    public string Provider { get; set; } = default!;

    public DateTime ExpireDateUtc { get; set; } = DateTime.UtcNow.AddDays(-1);
}