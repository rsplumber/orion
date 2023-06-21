namespace Core.Files;

public interface IFileLocationResolver
{
    ValueTask<List<FileLocation>> ResolveAsync(string link, CancellationToken cancellationToken = default);
}