namespace Core.Files.Events;

public sealed class FileLocationRefreshedEvent
{
    public const string EventName = "orion.file.location-refreshed";

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Provider { get; set; } = default!;
}