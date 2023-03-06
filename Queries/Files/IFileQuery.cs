namespace Queries.Files;

public interface IFileQuery
{
    Task<FileResponse> GetLinkAsync(Guid id, CancellationToken cancellationToken = default);
}