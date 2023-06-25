namespace Core;

public interface IFileProcessor
{
    Task<Stream> ProcessAsync(Stream file, Dictionary<string, string> configs = default!, CancellationToken cancellationToken = default);
}