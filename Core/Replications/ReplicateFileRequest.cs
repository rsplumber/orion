namespace Core.Replications;

public sealed record ReplicateFileRequest(Guid Id)
{
    public Guid FileId { get; init; } = default!;

    public string Provider { get; init; } = default!;
}