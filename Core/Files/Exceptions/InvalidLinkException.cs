namespace Core.Files.Exceptions;

public class InvalidLinkException : CoreException
{
    private const int DefaultCode = 400;
    private const string DefaultMessage = "InvalidLinkException";

    public InvalidLinkException() : base(DefaultCode, DefaultMessage)
    {
    }
}