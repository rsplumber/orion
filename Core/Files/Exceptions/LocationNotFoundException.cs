namespace Core.Files.Exceptions;

public class LocationNotFoundException : CoreException
{
    private const int DefaultCode = 404;
    private const string DefaultMessage = "Location Not found";

    public LocationNotFoundException() : base(DefaultCode, DefaultMessage)
    {
    }
}