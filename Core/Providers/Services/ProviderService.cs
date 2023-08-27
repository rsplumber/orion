namespace Core.Providers.Services;

internal sealed class ProviderService : IProviderService
{
    private readonly IProviderRepository _providerRepository;

    public ProviderService(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task AddAsync(Provider provider, CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.FindAsync(cancellationToken);

        switch (providers.Count)
        {
            case > 0 when providers.GroupBy(storage => storage.Name).Count() > 1:
                throw new ArgumentException("Duplicate storage name");
            case > 1 when providers.All(storage => !storage.Primary):
                throw new ArgumentException("You must set primary storage");
        }

        if (providers.Count(storage => storage.Primary) > 1)
        {
            var primaryStorages = providers.Where(storage => storage.Primary)
                .Select(s => s.Name)
                .ToArray();
            throw new ArgumentException($"{string.Join(",", primaryStorages)}: Cannot add more than 1 primary storage");
        }

        if (providers.Any(storage => storage is { Primary: true, Replication: true }))
        {
            var storage = providers.First(storage => storage is { Primary: true, Replication: true });
            throw new ArgumentException($"{storage.Name}: Primary and replication storage at the same time");
        }

        await _providerRepository.AddAsync(provider, cancellationToken);
    }

    public Task UpdateAsync(string providerName, Provider provider, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        // await _providerRepository.UpdateAsync(provider, cancellationToken);
    }
}