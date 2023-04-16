namespace Core.Files.Exceptions;

public class LocationNotFoundException : OrionException
{
    private const int DefaultCode = 404;
    private const string DefaultMessage = "Location Not found";

    public LocationNotFoundException() : base(DefaultCode, DefaultMessage)
    {
    }
}