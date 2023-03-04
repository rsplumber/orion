namespace Providers.Abstractions;

public class PutObject
{
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string BucketName { get; set; } = null!;
}