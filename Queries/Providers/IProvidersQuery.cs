namespace Queries.Providers;

public interface IProvidersQuery
{
    Task<List<ProviderResponse>> QueryAsync(CancellationToken cancellationToken = default);
}