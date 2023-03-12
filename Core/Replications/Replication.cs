using Core.Replications.Types;

namespace Core.Replications;

public class Replication
{
    public Guid Id { get; set; }

    public Guid FileId { get; init; } = default!;

    public string Provider { get; init; } = default!;
    public int Retry { get; private set; }

    public FileStatus Status { get; private set; } = FileStatus.Sending;

    public DateTime CreatedDateUtc { get; internal set; } = DateTime.UtcNow;

    public void IncrementRetry() => Retry += 1;

    public void Failed() => Status = FileStatus.Failed;

    public void Sent() => Status = FileStatus.Sent;
}