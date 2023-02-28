namespace Core.Files.Services;

public interface IFileService
{
    Task PutAsync(PutFileRequest req, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteFileRequest req, CancellationToken cancellationToken = default);

    Task GetAsync(GetFileRequest req, CancellationToken cancellationToken = default);
}