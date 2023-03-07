using Queries.Files;

namespace Data.Sql.Files;

internal sealed class FileQuery : IFileQuery
{
    private const string DefaultProvider = "local";

    private readonly ObjectStorageDbContext _dbContext;

    public FileQuery(ObjectStorageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<FileResponse>? GetLinkAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}