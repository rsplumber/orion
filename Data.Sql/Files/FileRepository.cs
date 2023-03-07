using Core.Files;
using Microsoft.EntityFrameworkCore;
using File = Core.Files.File;

namespace Data.Sql.Files;

public class FileRepository : IFileRepository
{
    private readonly ObjectStorageDbContext _dbContext;

    public FileRepository(ObjectStorageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(File entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Files.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(File entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Files.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(File entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Files.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<File?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Files
            .Include
                (f => f.Locations)
            .AsNoTracking()
            .Where(f => f.Id == id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}