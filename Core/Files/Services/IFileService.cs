namespace Core.Files.Services;

public interface IFileService
{
    Task<string> PutAsync(PutFileRequest req, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteFileRequest req, CancellationToken cancellationToken = default);

    Task<string> GetLocationAsync(GetFileRequest req, CancellationToken cancellationToken = default);
    
}