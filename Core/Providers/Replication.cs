using Core.Providers.Types;

namespace Core.Providers;

public sealed class Replication
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid FileId { get; init; }

    public string Provider { get; init; } = default!;

    public FileStatus Status { get; private set; } = FileStatus.Sent;

    public DateTime CreatedDateUtc { get; internal set; } = DateTime.UtcNow;

    public void Failed() => Status = FileStatus.Failed;

    public void Sent() => Status = FileStatus.Sent;
}