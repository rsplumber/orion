using Core.Files;
using Microsoft.EntityFrameworkCore;

namespace Data.EF.Files;

public sealed class FileLocationResolver : IFileLocationResolver
{
    private readonly OrionDbContext _dbContext;

    public FileLocationResolver(OrionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<List<FileLocation>> ResolveAsync(string link, CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == IdLink.Parse(link), cancellationToken: cancellationToken);
        return file is null ? new List<FileLocation>() : file.Locations;
    }
}