namespace Queries.Files;

public interface IFileQuery
{
    Task<FileResponse>? GetLinkAsync(string id, CancellationToken cancellationToken = default);
}