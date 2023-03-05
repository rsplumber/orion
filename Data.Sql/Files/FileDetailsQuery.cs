using Microsoft.EntityFrameworkCore;
using Queries.Files;

namespace Data.Sql.Files;

internal sealed class FileDetailsQuery : IFileQuery
{
    private readonly ObjectStorageDbContext _dbContext;

    public FileDetailsQuery(ObjectStorageDbContext dbContext)
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

        var locations = file.Locations.Select(location => new FileLocationResponse
        {
            Id = location.Id,
            Location = location.Location,
            Provider = location.Provider
        }).ToList();

        return new()
        {
            Link = locations.FirstOrDefault(response => response.Provider=="local").ToString()
        };
    }
}