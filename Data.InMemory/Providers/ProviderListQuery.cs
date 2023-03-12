using Core.Providers;
using Queries.Providers;

namespace Data.InMemory.Providers;

internal sealed class ProviderListQuery : IProviderListQuery
{
    private readonly IProviderRepository _providerRepository;

    public ProviderListQuery(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<List<ProviderResponse>> QueryAsync(CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.FindAsync(cancellationToken);
        return providers
            .Select(provider => new ProviderResponse
            {
                Name = provider.Name,
                Status = provider.Status.ToString(),
                Metas = provider.Metas
            }).ToList();
    }
}