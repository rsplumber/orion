namespace Core.Providers.Exceptions;

public class ProviderNotFoundException : CoreException
{
    private const int DefaultCode = 404;
    private const string DefaultMessage = "Provider Not found";

    public ProviderNotFoundException() : base(DefaultCode, DefaultMessage)
    {
    }
}