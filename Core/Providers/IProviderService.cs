namespace Core.Providers;

public interface IProviderService
{
    Task AddAsync(Provider provider, CancellationToken cancellationToken = default);

    Task UpdateAsync(string providerName, Provider provider, CancellationToken cancellationToken = default);
}