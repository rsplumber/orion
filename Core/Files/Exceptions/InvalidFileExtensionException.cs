namespace Core.Files.Exceptions;

public class InvalidFileExtensionException : CoreException
{
    private const int DefaultCode = 400;
    private const string DefaultMessage = "Invalid file extension";

    public InvalidFileExtensionException() : base(DefaultCode, DefaultMessage)
    {
    }
}