namespace Core.Files.Exceptions;

public class BucketNotFoundException : CoreException
{
    private const int DefaultCode = 404;
    private const string DefaultMessage = "Bucket not found";

    public BucketNotFoundException() : base(DefaultCode, DefaultMessage)
    {
    }
}