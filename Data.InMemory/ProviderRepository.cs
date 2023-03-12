using Core.Providers;

namespace Data.InMemory;

internal sealed class ProviderRepository : IProviderRepository
{
    private static readonly Dictionary<string, Provider> Providers = new();

    public Task<Provider?> FindByNameAsync(string providerName, CancellationToken cancellationToken = default)
    {
        Providers.TryGetValue(providerName, out var provider);
        return Task.FromResult(provider);
    }

    public Task<List<Provider>> FindAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Providers
            .Select(pair => pair.Value)
            .ToList());
    }

    public Task AddAsync(Provider provider, CancellationToken cancellationToken = default)
    {
        Providers.TryAdd(provider.Name, provider);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Provider provider, CancellationToken cancellationToken = default)
    {
        Providers.Remove(provider.Name);
        return AddAsync(provider, cancellationToken);
    }
}