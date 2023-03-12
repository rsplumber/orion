using Queries.Files;

namespace Data.Sql.Files;

internal sealed class FileQuery : IFileQuery
{
    private const string DefaultProvider = "local";

    private readonly OrionDbContext _dbContext;

    public FileQuery(OrionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<FileResponse>? GetLinkAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}