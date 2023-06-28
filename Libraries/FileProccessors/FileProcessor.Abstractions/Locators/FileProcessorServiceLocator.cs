namespace FileProcessor.Abstractions.Locators;

internal sealed class FileProcessorServiceLocator : IFileProcessorServiceLocator
{
    private readonly IEnumerable<IFileProcessor> _fileProcessors;

    public FileProcessorServiceLocator(IEnumerable<IFileProcessor> fileProcessors)
    {
        _fileProcessors = fileProcessors;
    }

    public Task<IFileProcessor> LocateAsync(string extension, CancellationToken cancellationToken = default)
    {
        var sanitizedExtension = SanitizeExtension();
        var locatedProcessor = _fileProcessors.FirstOrDefault(processor => processor.Type == _fileProcessors
            .Where(fileProcessor => fileProcessor.SupportedTypes.Any(v => v == sanitizedExtension))
            .Select(fileProcessor => fileProcessor.Type)
            .FirstOrDefault());
        if (locatedProcessor is null)
        {
            throw new InvalidProcessConfig($"Cannot process this type of file {extension}");
        }

        return Task.FromResult(locatedProcessor);

        string SanitizeExtension()
        {
            return extension.StartsWith(".") ? string.Join("", extension[1..]) : extension;
        }
    }
}