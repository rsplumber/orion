namespace Core.FileLocations;

public interface IFileLocationRepository
{
    Task AddAsync(FileLocation entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(FileLocation entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(FileLocation entity, CancellationToken cancellationToken = default);

    Task<FileLocation?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<FileLocation>?> FindByFileIdAsync(Guid fileId, CancellationToken cancellationToken = default);
}