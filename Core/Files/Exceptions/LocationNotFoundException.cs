namespace Core.Files.Exceptions;

public class LocationNotFoundException : ApplicationException
{
    public LocationNotFoundException() : base($"Location Not found")
    {
    }
}