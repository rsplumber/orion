using Core.Replications;
using Microsoft.EntityFrameworkCore;

namespace Data.Sql.Replications;

public class ReplicationRepository : IReplicationRepository
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

    public async Task<Replication?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Replications
            .AsNoTracking()
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}