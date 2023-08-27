namespace Data.Abstractions.Providers;

public interface IProvidersQuery
{
    Task<List<ProviderResponse>> QueryAsync(CancellationToken cancellationToken = default);
}