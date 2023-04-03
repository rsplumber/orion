using Core.Events;
using Core.Files.Exceptions;
using Core.Providers;
using CSharpVitamins;
using DotNetCore.CAP;
using Providers.Abstractions;

namespace Core.Files.Services;

public class FileService : IFileService
{
    private const string DefaultProvider = "minio";
    private readonly IFileRepository _fileRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly IEnumerable<IStorageService> _storageService;
    private readonly ICapPublisher _capPublisher;


    public FileService(IFileRepository fileRepository, ICapPublisher capPublisher, IEnumerable<IStorageService> storageService, IProviderRepository providerRepository)
    {
        _fileRepository = fileRepository;
        _capPublisher = capPublisher;
        _storageService = storageService;
        _providerRepository = providerRepository;
    }

    public async Task<PutFileResponse> PutAsync(Stream stream, PutFileRequest req, CancellationToken cancellationToken)
    {
        var localProvider = _storageService.FirstOrDefault(service => service.Provider == DefaultProvider);
        if (localProvider is null)
        {
            throw new ProviderNotFoundException();
        }

        var id = Guid.NewGuid();
        const string filePath = "default";
        var link = await localProvider.PutAsync(stream, new PutObject
        {
            Length = req.Lenght,
            Name = id + req.Extension,
            Path = filePath
        });

        var file = new File
        {
            Id = id,
            Name = id + req.Extension,
            Path = filePath,
            Metas = new Dictionary<string, string>
            {
                { "Extension", req.Extension }
            }
        };

        file.Add(new FileLocation
        {
            Link = link,
            Provider = localProvider.Provider,
        });

        await _fileRepository.AddAsync(file, cancellationToken);

        foreach (var provider in await _providerRepository.FindAsync(cancellationToken))
        {
            await _capPublisher.PublishAsync(ReplicateFileEvent.EventName + "." + provider.Name, new ReplicateFileEvent
            {
                FileId = file.Id,
                Provider = provider.Name
            }, cancellationToken: cancellationToken);
        }

        return new PutFileResponse(file.Id, GenerateLink(file.Id));
    }


    public async Task DeleteAsync(DeleteFileRequest req, CancellationToken cancellationToken = default)
    {
        var provider = _storageService.FirstOrDefault(service => service.Provider == DefaultProvider);

        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        var file = await _fileRepository.FindAsync(req.Id, cancellationToken);
        await provider.DeleteAsync(file!.Name, file.Path);
    }

    public async Task<string> GetLocationAsync(GetFileRequest req, CancellationToken cancellationToken = default)
    {
        var fileId = ExtractId(req.Link);
        var file = await _fileRepository.FindAsync(fileId, cancellationToken);

        if (file is null)
        {
            throw new FileNotFoundException();
        }

        if (file.Locations is null)
        {
            throw new LocationNotFoundException();
        }

        var fileLocation = file.Locations.First(location => location.Provider == "minio").Link;
        return fileLocation;
    }

    private static string GenerateLink(Guid fileId)
    {
        ShortGuid generatedLink = fileId;
        return generatedLink.Value;
    }

    private static Guid ExtractId(string link)
    {
        ShortGuid extractedLink = link;
        return extractedLink.Guid;
    }
}