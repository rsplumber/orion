namespace Core.Files.Services;

public interface IFileService
{
    Task PutAsync(PutFileRequest req, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteFileRequest req, CancellationToken cancellationToken = default);

    Task<MemoryStream> GetAsync(GetFileRequest req, CancellationToken cancellationToken = default);
    
    Task<string> EncryptLinkAsync(Guid fileId, CancellationToken cancellationToken = default);
    
    Task<Guid> DecryptLinkAsync(string link, CancellationToken cancellationToken = default);
}