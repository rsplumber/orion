namespace Core.Files;

public interface IFileRepository
{
    Task AddAsync(File entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(File entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(File entity, CancellationToken cancellationToken = default);

    Task<File?> FindAsync(Guid id, CancellationToken cancellationToken = default);
}