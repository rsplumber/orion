namespace Core.Providers.Exceptions;

public class ProviderNameExistsException : CoreException
{
    private const int DefaultCode = 400;

    public ProviderNameExistsException(string name) : base(DefaultCode, $"Provider {name} exists")
    {
    }
}