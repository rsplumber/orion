namespace Providers.Abstractions;

public sealed record PutObject
{
    public string Name { get; init; } = default!;
    
    public string Path { get; set; } = default!;
    public long Length { get; init; }
    
    
    
}