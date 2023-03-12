namespace Data.InMemory.Providers.Exceptions;

public class ProviderNotFoundException : ApplicationException
{
    public ProviderNotFoundException() : base($"Provider Not found")
    {
    }
}