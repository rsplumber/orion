namespace Core.Files.Exceptions;

public class ProviderNotFoundException : OrionException
{
    private const int DefaultCode = 404;
    private const string DefaultMessage = "Provider Not found";

    public ProviderNotFoundException() : base(DefaultCode, DefaultMessage)
    {
    }
}