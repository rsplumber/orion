using Core.Providers.Types;

namespace Core.Providers;

public class Provider
{
    public string Name { get; set; } = default!;

    public ProviderStatus Status { get; set; } = ProviderStatus.Enable;

    public Dictionary<string, string> Metas { get; set; } = new();
}