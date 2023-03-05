namespace Queries.Files;

public interface IFileListQuery
{
    Task<List<FileResponse>> QueryAsync(CancellationToken cancellationToken = default);
}