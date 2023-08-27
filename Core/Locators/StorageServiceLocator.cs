using Core.Providers;
using Core.Providers.Types;
using Storages.Abstractions;

namespace Core.Locators;

public class StorageServiceLocator : IStorageServiceLocator
{
    private readonly IEnumerable<IStorageService> _storageServices;
    private readonly IProviderRepository _providerRepository;

    public StorageServiceLocator(IEnumerable<IStorageService> storageServices, IProviderRepository providerRepository)
    {
        _storageServices = storageServices;
        _providerRepository = providerRepository;
    }

    public async Task<IStorageService?> LocatePrimaryAsync(CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.FindAsync(cancellationToken);
        var primaryProvider = providers.First(provider => provider is { Primary: true, Status: ProviderStatus.Enable });
        return _storageServices.FirstOrDefault(service => service.Name == primaryProvider.Name);
    }

    public async Task<IStorageService?> LocateAsync(string providerName, CancellationToken cancellationToken = default)
    {
        var providers = await _providerRepository.FindAsync(cancellationToken);
        var selectedProvider = providers.First(provider => provider.Name == providerName && provider.Status == ProviderStatus.Enable);
        return _storageServices.FirstOrDefault(service => service.Name == selectedProvider.Name);
    }
}