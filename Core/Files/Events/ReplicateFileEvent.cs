namespace Core.Files.Events;

public class ReplicateFileEvent
{
    public const string EventName = "file_replicate";

    public string Name { get; set; }
}