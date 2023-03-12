namespace Queries.Providers;

public interface IProviderListQuery
{
    Task<List<ProviderResponse>> QueryAsync(CancellationToken cancellationToken = default);
}