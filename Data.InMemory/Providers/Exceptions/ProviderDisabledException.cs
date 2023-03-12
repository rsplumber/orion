namespace Data.InMemory.Providers.Exceptions;

public class ProviderDisabledException : ApplicationException
{
    public ProviderDisabledException() : base($"Provider is not enabled")
    {
    }
}