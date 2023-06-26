namespace Core.Providers.Exceptions;

public class ProviderDisabledException : CoreException
{
    private const int DefaultCode = 400;
    private const string DefaultMessage = "Provider is disabled";

    public ProviderDisabledException() : base(DefaultCode, DefaultMessage)
    {
    }
}