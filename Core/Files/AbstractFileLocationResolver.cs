namespace Core.Files;

public abstract class AbstractFileLocationResolver : IFileLocationResolver
{
    public abstract ValueTask<List<FileLocation>> ResolveAsync(string link, CancellationToken cancellationToken = default);
}