using Core.Files.Exceptions;
using Core.Providers;
using Core.Providers.Events;
using Core.Providers.Types;
using DotNetCore.CAP;

namespace Core.Files.Services;

internal sealed class PutFileService : IPutFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly IStorageServiceLocator _storageServiceLocator;
    private readonly ICapPublisher _capPublisher;
    private readonly IFileProcessor _imageProcessor;

    public PutFileService(IFileRepository fileRepository, IProviderRepository providerRepository, ICapPublisher capPublisher, IFileProcessor imageProcessor, IStorageServiceLocator storageServiceLocator)
    {
        _fileRepository = fileRepository;
        _providerRepository = providerRepository;
        _capPublisher = capPublisher;
        _imageProcessor = imageProcessor;
        _storageServiceLocator = storageServiceLocator;
    }

    public async Task<PutFileResponse> PutAsync(Stream stream, PutFileRequest req, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var filePath = $"{req.OwnerId}/{req.Path}";
        var fileExtension = GetFileExtension(req.Name);

        if (req.Configs is not null && req.Configs.Count > 0)
        {
            var processor = ResolveFileProcessor(fileExtension);
            var processedResponse = await processor.ProcessAsync(stream, req.Configs, cancellationToken);
            stream = processedResponse.Content;
            fileExtension = GetFileExtension(processedResponse.Name);
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

        var providers = await _providerRepository.FindAsync(cancellationToken);
        foreach (var provider in providers.Where(provider => provider is
                 {
                     Primary: false,
                     Replication: true,
                     Status: ProviderStatus.Enable
                 }))
        {
            await _capPublisher.PublishAsync($"{ReplicateFileEvent.EventName}.{provider.Name}", new ReplicateFileEvent
            {
                FileId = file.Id,
                Provider = provider.Name
            }, cancellationToken: cancellationToken);
        }

        return new PutFileResponse(file.Id, IdLink.From(file.Id));

        string GetFileExtension(string name) => Path.HasExtension(name) ? Path.GetExtension(name) : throw new InvalidFileExtensionException();
    }

    private IFileProcessor ResolveFileProcessor(string extension)
    {
        return SanitizeExtension() switch
        {
            "jpg" or "jpeg" or "png" or "tiff" or "tif"
                or "webp" or "web" => _imageProcessor,
            _ => throw new ArgumentOutOfRangeException()
        };

        string SanitizeExtension()
        {
            return extension.StartsWith(".") ? string.Join("", extension[1..]) : extension;
        }
    }
}