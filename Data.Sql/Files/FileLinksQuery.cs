using Microsoft.EntityFrameworkCore;
using Queries.Files;

namespace Data.Sql.Files;

internal sealed class FileLinksQuery : IFileQuery
{
    private const string DefaultProvider = "local";
    private readonly ObjectStorageDbContext _dbContext;

    public FileLinksQuery(ObjectStorageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FileResponse> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.Files
            .FirstOrDefaultAsync(model => model.Id == id, cancellationToken);

        if (file is null)
        {
            throw new FileNotFoundException();
        }

        return new FileResponse
        {
            Link = file.Locations.FirstOrDefault(response => response.Provider == DefaultProvider)!.ToString()!
        };
    }
}