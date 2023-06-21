namespace Core.Storages;

public sealed record FileLink
{
    public required string Url { get; init; } = default!;

    public DateTime? ExpireDateTimeUtc { get; init; }
}