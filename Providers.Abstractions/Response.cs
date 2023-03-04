namespace Providers.Abstractions;

public class S3ResponseDto
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = default!;
}