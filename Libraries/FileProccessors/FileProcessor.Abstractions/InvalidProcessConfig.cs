namespace FileProcessor.Abstractions;

public sealed class InvalidProcessConfig : ApplicationException
{
    public InvalidProcessConfig(string message) : base(message)
    {
    }
}