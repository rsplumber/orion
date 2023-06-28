namespace FileProcessor.Abstractions;

public interface IFileProcessorServiceLocator
{
    Task<IFileProcessor> LocateAsync(string extension, CancellationToken cancellationToken = default);
}