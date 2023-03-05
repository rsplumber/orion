namespace Core.Files.Exceptions;

public class ProviderNotFoundException : ApplicationException
{
    public ProviderNotFoundException() : base($"Provider Not found")
    {
    }
}