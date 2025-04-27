namespace Storages.Abstractions;

public interface IStorageService
{
    public string Name { get; }

    ValueTask<FileLink> PutAsync(Stream stream, string path, string name);

    ValueTask GetAsync(string path, string name, Action<Stream> stream);

    ValueTask<FileLink> RefreshLinkAsync(string path, string name);

    ValueTask DeleteAsync(string path, string name);
}