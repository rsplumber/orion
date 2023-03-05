using Core.Files.Events;
using Core.Files.Exceptions;
using DotNetCore.CAP;
using Providers.Abstractions;

namespace Core.Files.Services;

public class FileService : IFileService
{
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
        var provider = _storageService.FirstOrDefault(service => service.ProviderName == "minio");
        if (provider is null)
        {
            throw new ProviderNotFoundException();
        }

        var fileAddress = await provider.PutAsync(new PutObject
        {
            Name = req.Name,
            ContentType = req.ContentType,
            Path = req.FilePath,
        });

        System.IO.File.Delete(req.FilePath);

        await _fileRepository.AddAsync(new File
        {
            Name = req.Name,
            Metas = new Dictionary<string, string>
            {
                {"Extension", req.Extension},
                {"ContentType", req.ContentType}
            },
            Locations = new Dictionary<string, string>()
            {
                {"local", fileAddress}
            }
        }, cancellationToken);


        //todo put to MinIo
        //todo put MinIo fileAddress to file.location

        await _capPublisher.PublishAsync(ReplicateFileEvent.EventName, new ReplicateFileEvent
        {
            // Id = file.Id,
        }, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(DeleteFileRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<MemoryStream> GetAsync(GetFileRequest req, CancellationToken cancellationToken = default)
    {
        var provider = _storageService.FirstOrDefault(service => service.ProviderName == "minio");

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