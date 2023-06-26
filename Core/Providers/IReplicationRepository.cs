namespace Core.Providers;

public interface IReplicationRepository
{
    Task AddAsync(Replication entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(Replication entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Replication entity, CancellationToken cancellationToken = default);

    Task<Replication?> FindAsync(Guid id, CancellationToken cancellationToken = default);
}