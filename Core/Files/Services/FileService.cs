using Core.Files.Events;
using Core.Files.Exceptions;
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

    public async Task PutAsync(PutFileRequest req, CancellationToken cancellationToken)
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
        // System.IO.File.Delete(req.FilePath);


        await _capPublisher.PublishAsync(ReplicateFileEvent.EventName, new ReplicateFileEvent
        {
            Id = file.Id,
        }, cancellationToken: cancellationToken);
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

    public async Task<MemoryStream> GetAsync(GetFileRequest req, CancellationToken cancellationToken = default)
    {
        var provider = _storageService.FirstOrDefault(service => service.ProviderName == DefaultProvider);

        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        return await provider.GetAsync(new GetObject()
        {
            Name = req.Name,
        });
    }
}