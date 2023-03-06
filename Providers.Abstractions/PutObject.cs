namespace Providers.Abstractions;

public sealed record PutObject
{
    public string Name { get; init; } = null!;
    public string Path { get; init; } = null!;
    public string ContentType { get; init; } = null!;
}