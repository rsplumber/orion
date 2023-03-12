namespace Core.Providers;

public interface IProviderRepository
{
    Task<List<Provider>> FindAsync(CancellationToken cancellationToken = default);

    Task<Provider?> FindByNameAsync(string providerName, CancellationToken cancellationToken = default);

    Task AddAsync(Provider provider, CancellationToken cancellationToken = default);

    Task UpdateAsync(Provider provider, CancellationToken cancellationToken = default);
}