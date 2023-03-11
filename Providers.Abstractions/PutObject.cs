namespace Providers.Abstractions;

public sealed record PutObject
{
    public string Name { get; init; } = null!;
    public long Length { get; init; } = default;
    public string ContentType { get; init; } = null!;
}