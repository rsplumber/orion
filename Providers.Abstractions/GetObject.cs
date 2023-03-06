namespace Providers.Abstractions;

public sealed record GetObject
{
    public string Name { get; init; } = null!;
}