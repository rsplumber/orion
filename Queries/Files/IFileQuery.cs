namespace Queries.Files;

public interface IFileQuery
{
    Task<FileResponse> GetAsync(Guid id, CancellationToken cancellationToken = default);
}