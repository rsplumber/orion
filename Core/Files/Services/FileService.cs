using Core.Files.Events;
using Core.Files.Exceptions;
using Core.Tools.Cryptography;
using DotNetCore.CAP;
using Providers.Abstractions;

namespace Core.Files.Services;

public class FileService : IFileService
{
    private const string DefaultProvider = "minio";
    private readonly IFileRepository _fileRepository;
    private readonly IEnumerable<IStorageService> _storageService;
    private readonly ICapPublisher _capPublisher;


    public FileService(IFileRepository fileRepository, ICapPublisher capPublisher, IEnumerable<IStorageService> storageService)
    {
        _fileRepository = fileRepository;
        _capPublisher = capPublisher;
        _storageService = storageService;
    }

    public async Task<string> PutAsync(PutFileRequest req, CancellationToken cancellationToken)
    {
        var provider = _storageService.FirstOrDefault(service => service.ProviderName == DefaultProvider);
        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        var link = await provider.PutAsync(new PutObject
        {
            Name = req.Name,
            ContentType = req.ContentType,
            Path = req.FilePath,
        });

        var file = new File
        {
            Name = req.Name,
            Metas = new Dictionary<string, string>
            {
                {"Extension", req.Extension},
                {"ContentType", req.ContentType}
            },
        };

        file.Add(new FileLocation
        {
            Location = link,
            Filename = req.Name,
            Provider = "local",
        });

        await _fileRepository.AddAsync(file, cancellationToken);


        // todo delete file from local bug should be fixed

        await _capPublisher.PublishAsync(ReplicateFileEvent.EventName, new ReplicateFileEvent
        {
            Id = file.Id,
        }, cancellationToken: cancellationToken);
        return GenerateLink(file.Id);
    }

    public async Task DeleteAsync(DeleteFileRequest req, CancellationToken cancellationToken = default)
    {
        var provider = _storageService.FirstOrDefault(service => service.ProviderName == DefaultProvider);

        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        var fileName = (await _fileRepository.FindAsync(req.Id, cancellationToken))!.Name;
        await provider.DeleteAsync(new DeleteObject
        {
            Name = fileName
        });
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

        return file.Locations.First(location => location.Provider == "local").Location;
    }

    private static string GenerateLink(Guid fileId)
    {
        return Cryptography.AES.Encrypt(
            fileId.ToString(), "default"
        );
    }

    private static Guid ExtractId(string link)
    {
        var extracted = Cryptography.AES.Decrypt(
            link, "default");
        return Guid.Parse(extracted);
    }
}