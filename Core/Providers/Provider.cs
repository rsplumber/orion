using Core.Providers.Types;

namespace Core.Providers;

public sealed class Provider
{
    public string Name { get; set; } = default!;

    public bool Primary { get; set; }

    public bool Replication { get; set; }

    public ProviderStatus Status { get; set; } = ProviderStatus.Enable;

    public Dictionary<string, string> Metas { get; set; } = new();

    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;
}