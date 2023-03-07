namespace Core.Files.Services;

public interface IFileService
{
    Task<string> PutAsync(Stream stream, PutFileRequest req, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteFileRequest req, CancellationToken cancellationToken = default);

    Task<string> GetLocationAsync(GetFileRequest req, CancellationToken cancellationToken = default);
}