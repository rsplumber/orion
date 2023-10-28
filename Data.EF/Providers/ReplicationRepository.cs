using Core.Providers;
using Microsoft.EntityFrameworkCore;

namespace Data.EF.Providers;

internal sealed class ReplicationRepository : IReplicationRepository
{
    private readonly OrionDbContext _dbContext;

    public ReplicationRepository(OrionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Replication entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Replications.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Replication entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Replications.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Replication entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Replications.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Replication?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Replications.FirstOrDefaultAsync(r => r.Id == id, cancellationToken: cancellationToken);
    }
}