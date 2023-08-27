using Core.Providers.Exceptions;
using Data.Abstractions.Providers;
using Microsoft.EntityFrameworkCore;

namespace Data.EF.Providers;

internal sealed class ProviderQuery : IProviderQuery
{
    private readonly OrionDbContext _dbContext;

    public ProviderQuery(OrionDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<ProviderResponse> QueryAsync(string name, CancellationToken cancellationToken = default)
    {
        var provider = await _dbContext.Providers.FirstOrDefaultAsync(provider => provider.Name == name, cancellationToken);
        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        return new ProviderResponse
        {
            Name = provider.Name,
            Status = provider.Status.ToString(),
            Metas = provider.Metas
        };
    }
}