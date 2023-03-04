namespace Providers.Abstractions;

public class GetObject
{
    public string Name { get; set; } = null!;
    public string BucketName { get; set; } = null!;
    public MemoryStream Stream { get; set; } = null!;
}