using Microsoft.EntityFrameworkCore;
using Queries.Files;

namespace Data.Sql.Files;

internal sealed class FileListQuery : IFileListQuery
{
    private readonly ObjectStorageDbContext _dbContext;

    public FileListQuery(ObjectStorageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<FileResponse>> QueryAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Files
            .Select(file => new FileResponse
            {
                Id = file.Id,
                Locations = file.Locations.Select(location => new FileLocationResponse
                {
                    Id = location.Id,
                    Location = location.Location,
                    Provider = location.Provider
                }).ToList(),
                Metas = file.Metas,
                CreatedDateUtc = file.CreatedDateUtc
            })
            .ToListAsync(cancellationToken: cancellationToken);
    }
}