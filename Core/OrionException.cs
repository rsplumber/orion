namespace Core;

public class OrionException : ApplicationException
{
    public OrionException(int code, string message)
    {
        Code = code;
        Message = message;
    }

    public int Code { get; }

    public new string Message { get; }
}