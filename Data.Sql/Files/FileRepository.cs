using Core.Files;
using File = Core.Files.File;

namespace Data.Sql.Files;

public class FileRepository : IFileRepository
{
    public Task AddAsync(File entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(File entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(File entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<File?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}