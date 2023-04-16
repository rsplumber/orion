using Core;

namespace Data.InMemory.Providers.Exceptions;

public class ProviderDisabledException : OrionException
{
    private const int DefaultCode = 400;
    private const string DefaultMessage = "Provider is disabled";

    public ProviderDisabledException() : base(DefaultCode, DefaultMessage)
    {
    }
}