namespace Core.Files;

public interface IFileLocationService
{
    Task<string?> GetAsync(string fileLink, CancellationToken cancellationToken = default);
}