namespace Providers.Abstractions;

public interface IStorageService
{
    public string ProviderName { get; }
    public string AccessKey { get; }
    public string SecretKey { get; }

    Task<string> PutAsync(PutObject obj);

    Task<string> GetAsync(GetObject obj);

    Task<string> DeleteAsync(DeleteObject obj);
}