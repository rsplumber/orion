namespace Providers.Abstractions;

public interface IStorageService
{
    Task<string> PutAsync(Credential credential, PutObject obj);

    Task<string> GetAsync(GetObject obj);

    Task<string> DeleteAsync(DeleteObject obj);
}