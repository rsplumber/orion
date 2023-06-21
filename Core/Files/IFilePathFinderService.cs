namespace Core.Files;

public interface IFilePathFinderService
{
    Task<string?> GetAbsolutePathAsync(string fileLink, CancellationToken cancellationToken = default);
}