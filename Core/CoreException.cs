namespace Core;

public class CoreException : ApplicationException
{
    public CoreException(int code, string message)
    {
        Code = code;
        Message = message;
    }

    public int Code { get; }

    public new string Message { get; }
}