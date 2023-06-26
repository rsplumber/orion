namespace Storages.Abstractions;

public sealed record FileLink
{
    public required string Url { get; init; } = default!;

    public DateTime? ExpireDateTimeUtc { get; init; }
}