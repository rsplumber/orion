using Storages.Abstractions;

namespace Core;

public interface IStorageServiceLocator
{
    Task<IStorageService?> LocatePrimaryAsync(CancellationToken cancellationToken = default);

    Task<IStorageService?> LocateAsync(string providerName, CancellationToken cancellationToken = default);
}