using Core.Files.Exceptions;
using Core.Providers;
using Core.Providers.Events;
using Core.Providers.Exceptions;
using Core.Providers.Types;
using DotNetCore.CAP;
using FileProcessor.Abstractions;

namespace Core.Files.Services;

internal sealed class PutFileService : IPutFileService
{
    private readonly IBucketRepository _bucketRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly IStorageServiceLocator _storageServiceLocator;
    private readonly IFileProcessorServiceLocator _fileProcessorServiceLocator;
    private readonly ICapPublisher _capPublisher;

    public PutFileService(
        IFileRepository fileRepository,
        IProviderRepository providerRepository,
        ICapPublisher capPublisher,
        IStorageServiceLocator storageServiceLocator,
        IFileProcessorServiceLocator fileProcessorServiceLocator,
        IBucketRepository bucketRepository)
    {
        _fileRepository = fileRepository;
        _providerRepository = providerRepository;
        _capPublisher = capPublisher;
        _storageServiceLocator = storageServiceLocator;
        _fileProcessorServiceLocator = fileProcessorServiceLocator;
        _bucketRepository = bucketRepository;
    }

    public async Task<PutFileResponse> PutAsync(Stream stream, PutFileRequest req, CancellationToken cancellationToken = default)
    {
        var bucket = await _bucketRepository.FindAsync(req.BucketId, cancellationToken).ConfigureAwait(false);
        if (bucket is null)
            throw new BucketNotFoundException();

        var id = Guid.NewGuid();
        var filePath = BuildFilePath(bucket.Name, req.Path);
        var fileExtension = req.Extension;

        if (req.HasConfig())
        {
            var processor = await _fileProcessorServiceLocator.LocateAsync(fileExtension, cancellationToken).ConfigureAwait(false);
            var processed = await processor.ProcessAsync(stream, req.Configs!, cancellationToken).ConfigureAwait(false);
            await stream.DisposeAsync().ConfigureAwait(false);
            stream = processed.Content;
            fileExtension = processed.Extension;
        }

        var fileName = $"{id}{fileExtension}";
        var storageService = await _storageServiceLocator.LocatePrimaryAsync(cancellationToken).ConfigureAwait(false);
        if (storageService is null)
            throw new ProviderNotFoundException();

        using var link = await storageService.PutAsync(stream, filePath, fileName).ConfigureAwait(false);

        var file = new File
        {
            Id = id,
            Name = fileName,
            Path = filePath,
            Bucket = bucket,
            Metas = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "Extension", fileExtension }
            }
        };

        file.Add(new FileLocation
        {
            Link = link.Url,
            ExpireDateUtc = link.ExpireDateTimeUtc,
            Provider = storageService.Name
        });

        await _fileRepository.AddAsync(file, cancellationToken).ConfigureAwait(false);
        await stream.DisposeAsync().ConfigureAwait(false);

        _ = ReplicateFileAsync(file.Id, cancellationToken);

        return new PutFileResponse(file.Id, IdLink.From(file.Id));
    }

    private static string BuildFilePath(string bucketName, string path) =>
        string.IsNullOrEmpty(path)
            ? bucketName
            : $"{bucketName}/{path}";

    private async Task ReplicateFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        var providers = await _providerRepository.FindAsync(cancellationToken).ConfigureAwait(false);

        foreach (var provider in providers)
        {
            if (!provider.Primary && provider.Replication && provider.Status == ProviderStatus.Enable)
            {
                _ = _capPublisher.PublishAsync(
                    $"{ReplicateFileEvent.EventName}.{provider.Name}",
                    new ReplicateFileEvent
                    {
                        FileId = fileId,
                        Provider = provider.Name
                    },
                    cancellationToken: cancellationToken);
            }
        }
    }
}