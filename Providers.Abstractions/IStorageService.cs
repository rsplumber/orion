namespace Providers.Abstractions;

public interface IStorageService
{
    public string ProviderName { get; }

    Task<string> PutAsync(Stream stream, PutObject obj);

    Task<MemoryStream> GetAsync(GetObject obj);

    Task DeleteAsync(DeleteObject obj);
}