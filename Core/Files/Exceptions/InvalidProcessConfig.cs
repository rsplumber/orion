namespace Core.Files.Exceptions;

public class InvalidProcessConfig : CoreException
{
    private const int DefaultCode = 400;

    public InvalidProcessConfig(string message) : base(DefaultCode, message)
    {
    }
}