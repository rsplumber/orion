namespace Queries.Providers;

public interface IProviderQuery
{
    Task<ProviderResponse> QueryAsync(string name, CancellationToken cancellationToken = default);
}