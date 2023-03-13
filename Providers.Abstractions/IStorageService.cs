namespace Providers.Abstractions;

public interface IStorageService
{
    public string Provider { get; }

    Task<string> PutAsync(Stream stream, PutObject obj);

    Task GetAsync(string path, string name, Action<Stream> stream);

    Task DeleteAsync(string path, string name);
}