namespace FileProcessor.Abstractions;

public interface IFileProcessor
{
    public string Type { get; }

    public string Name { get; }

    public IEnumerable<string> SupportedTypes { get; }

    Task<ProcessedResponse> ProcessAsync(Stream file, Dictionary<string, string> configs = default!, CancellationToken cancellationToken = default);
}

public sealed record ProcessedResponse
{
    public required Stream Content { get; init; }

    public required string Name { get; init; }

    public long ElapsedMilliseconds { get; init; }

    public string Extension => Path.HasExtension(Name) ? Path.GetExtension(Name) : string.Empty;
}