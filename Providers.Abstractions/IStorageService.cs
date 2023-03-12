namespace Providers.Abstractions;

public interface IStorageService
{
    public string Provider { get; }

    Task<string> PutAsync(Stream stream, PutObject obj);

    Task<MemoryStream> GetAsync(string path, string name);

    Task DeleteAsync(string path, string name);
}