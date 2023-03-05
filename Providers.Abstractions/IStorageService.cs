namespace Providers.Abstractions;

public interface IStorageService
{
    public string ProviderName { get; }


    Task<string> PutAsync(PutObject obj);

    Task<MemoryStream> GetAsync(GetObject obj);

    Task<string> DeleteAsync(DeleteObject obj);
}