namespace Core.Files;

public interface IBucketRepository
{
    Task<Bucket?> FindAsync(Guid id, CancellationToken cancellationToken = default);
}