using Core.Files;
using Microsoft.EntityFrameworkCore;

namespace Data.Sql.Files;

public sealed class FileLocationResolver : AbstractFileLocationResolver, IFileLocationResolver
{
    private readonly OrionDbContext _dbContext;

    public FileLocationResolver(OrionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async ValueTask<List<FileLocation>> ResolveAsync(string link, CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == IdLink.Parse(link), cancellationToken: cancellationToken);
        return file is null ? new List<FileLocation>() : file.Locations;
    }
}