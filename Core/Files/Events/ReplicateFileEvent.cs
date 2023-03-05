namespace Core.Files.Events;

public class ReplicateFileEvent
{
    public const string EventName = "file_replicate";

    public Guid Id { get; set; }
}