namespace Queries.Providers;

public interface IProviderDetailsQuery
{
    Task<ProviderResponse> QueryAsync(string name, CancellationToken cancellationToken = default);
}