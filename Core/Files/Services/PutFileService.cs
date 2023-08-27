using Core.Providers;
using Core.Providers.Events;
using Core.Providers.Types;
using DotNetCore.CAP;
using FileProcessor.Abstractions;

namespace Core.Files.Services;

internal sealed class PutFileService : IPutFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly IStorageServiceLocator _storageServiceLocator;
    private readonly IFileProcessorServiceLocator _fileProcessorServiceLocator;
    private readonly ICapPublisher _capPublisher;

    public PutFileService(IFileRepository fileRepository, IProviderRepository providerRepository, ICapPublisher capPublisher, IStorageServiceLocator storageServiceLocator, IFileProcessorServiceLocator fileProcessorServiceLocator)
    {
        _fileRepository = fileRepository;
        _providerRepository = providerRepository;
        _capPublisher = capPublisher;
        _storageServiceLocator = storageServiceLocator;
        _fileProcessorServiceLocator = fileProcessorServiceLocator;
    }

    public async Task<PutFileResponse> PutAsync(Stream stream, PutFileRequest req, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var filePath = $"{req.OwnerId}/{req.Path}";
        var fileExtension = req.Extension;

        if (req.HasConfig())
        {
            var processor = await _fileProcessorServiceLocator.LocateAsync(fileExtension, cancellationToken);
            var processedResponse = await processor.ProcessAsync(stream, req.Configs!, cancellationToken);
            stream = processedResponse.Content;
            fileExtension = processedResponse.Extension;
        }

        var fileName = $"{id}{fileExtension}";
        var storageService = await _storageServiceLocator.LocatePrimaryAsync(cancellationToken);
        var link = await storageService.PutAsync(stream, filePath, fileName);
        var file = new File
        {
            Id = id,
            Name = fileName,
            Path = filePath,
            OwnerId = req.OwnerId,
            Metas = new Dictionary<string, string>
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

        await _fileRepository.AddAsync(file, cancellationToken);
        await stream.DisposeAsync();
        
        _ = ReplicateAsync();

        return new PutFileResponse(file.Id, IdLink.From(file.Id));

        async Task ReplicateAsync()
        {
            var providers = await _providerRepository.FindAsync(cancellationToken);
            foreach (var provider in providers.Where(provider => provider is
                     {
                         Primary: false,
                         Replication: true,
                         Status: ProviderStatus.Enable
                     }))
            {
                _ = _capPublisher.PublishAsync($"{ReplicateFileEvent.EventName}.{provider.Name}", new ReplicateFileEvent
                {
                    FileId = file.Id,
                    Provider = provider.Name
                }, cancellationToken: cancellationToken);
            }
        }
    }
}