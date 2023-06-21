namespace Core.Files;

public interface ILocationSelector
{
    Task<FileLocation?> SelectAsync(List<FileLocation> locations, CancellationToken cancellationToken = default);
}