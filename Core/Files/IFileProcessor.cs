namespace Core.Files;

public interface IFileProcessor
{
    Task<ProcessedResponse> ProcessAsync(Stream file, Dictionary<string, string> configs = default!, CancellationToken cancellationToken = default);
}

public sealed record ProcessedResponse
{
    public required Stream Content { get; set; }

    public required string Name { get; set; }

    public long ElapsedMilliseconds { get; set; }
}