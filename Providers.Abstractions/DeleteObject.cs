namespace Providers.Abstractions;

public sealed record DeleteObject
{
    public string Name { get; init; } = null!;
}