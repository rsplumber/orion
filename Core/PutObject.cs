namespace Core;

public sealed record PutObject
{
    public string Name { get; init; } = default!;

    public string Path { get; init; } = default!;
   
    public long Length { get; init; }
}