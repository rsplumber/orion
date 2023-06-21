namespace Core.Storages;

public interface IStorageService
{
    public string Provider { get; }

    Task<FileLink> PutAsync(Stream stream, string name, string bucketName);

    Task GetAsync(string path, string name, Action<Stream> stream);

    Task<FileLink> RefreshLinkAsync(string path, string name);

    Task DeleteAsync(string path, string name);
}