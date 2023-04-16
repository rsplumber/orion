using Core;

namespace Data.InMemory.Providers.Exceptions;

public class ProviderNameExistsException : OrionException
{
    private const int DefaultCode = 400;

    public ProviderNameExistsException(string name) : base(DefaultCode, $"Provider {name} exists")
    {
    }
}