using Core.Files;
using Microsoft.EntityFrameworkCore;

namespace Data.EF.Files;

internal sealed class BucketRepository : IBucketRepository
{
    private readonly OrionDbContext _dbContext;

    public BucketRepository(OrionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Bucket?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Buckets.FirstOrDefaultAsync(bucket => bucket.Id == id, cancellationToken);
    }
}