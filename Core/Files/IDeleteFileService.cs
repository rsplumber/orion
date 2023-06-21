namespace Core.Files;

public interface IDeleteFileService
{
    Task DeleteAsync(string fileLink, CancellationToken cancellationToken = default);
}