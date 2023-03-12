namespace Data.InMemory.Providers.Exceptions;

public class ProviderNameExistsException : ApplicationException
{
    public ProviderNameExistsException(string name) : base($"Provider {name} exists")
    {
    }
}