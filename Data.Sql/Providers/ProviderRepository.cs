using Core.Providers;
using Microsoft.EntityFrameworkCore;

namespace Data.Sql.Providers;

internal sealed class ProviderRepository : IProviderRepository
{
    private readonly OrionDbContext _dbContext;

    public ProviderRepository(OrionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Provider?> FindByNameAsync(string providerName, CancellationToken cancellationToken = default)
    {
        return _dbContext.Providers.FirstOrDefaultAsync(provider => provider.Name == providerName, cancellationToken);
    }

    public Task<List<Provider>> FindAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Providers.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Provider provider, CancellationToken cancellationToken = default)
    {
        await _dbContext.Providers.AddAsync(provider, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Provider provider, CancellationToken cancellationToken = default)
    {
        _dbContext.Providers.Update(provider);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}