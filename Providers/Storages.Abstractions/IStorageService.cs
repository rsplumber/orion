namespace Storages.Abstractions;

public interface IStorageService
{
    public string Name { get; }

    Task<FileLink> PutAsync(Stream stream, string path, string name);

    Task GetAsync(string path, string name, Action<Stream> stream);

    Task<FileLink> RefreshLinkAsync(string path, string name);

    Task DeleteAsync(string path, string name);
}