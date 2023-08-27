namespace Data.Abstractions.Providers;

public sealed record ProviderResponse
{
    public string Name { get; init; } = default!;

    public string Status { get; init; } = default!;

    public Dictionary<string, string> Metas { get; init; } = new();
}