namespace Core;

public interface IStorageService
{
    public string Provider { get; }

    Task<Link> PutAsync(Stream stream, PutObject obj);

    Task GetAsync(string path, string name, Action<Stream> stream);

    Task<Link> RefreshLinkAsync(string path, string name);

    Task DeleteAsync(string path, string name);
}