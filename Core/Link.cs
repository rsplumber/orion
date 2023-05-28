namespace Core;

public sealed record Link
{
    public string Url { get; init; } = default!;

    public DateTime ExpireDateTimeUtc { get; init; } = default!;
}