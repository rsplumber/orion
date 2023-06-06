namespace Core;

public sealed record PutObject
{
    public string Name { get; init; } = default!;

    public string BucketName { get; init; } = default!;
   
    public long Length { get; init; }
}