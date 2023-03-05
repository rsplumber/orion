namespace Queries.Files;

public interface IFileDetailsQuery
{
    Task<FileResponse> QueryAsync(Guid id, CancellationToken cancellationToken = default);
}