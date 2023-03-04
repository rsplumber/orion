using Core.Files.Events;
using DotNetCore.CAP;
using Providers.Abstractions;

namespace Core.Files.Services;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IStorageService _storageService;
    private readonly ICapPublisher _capPublisher;


    public FileService(IFileRepository fileRepository, ICapPublisher capPublisher, IStorageService storageService)
    {
        _fileRepository = fileRepository;
        _capPublisher = capPublisher;
        _storageService = storageService;
    }

    public async Task PutAsync(PutFileRequest req, CancellationToken cancellationToken)
    {
        await using (var fs = System.IO.File.Open(req.FilePath, FileMode.Open))
        {
            await _fileRepository.AddAsync(new File
            {
                Name = req.Name,
                Metas = new Dictionary<string, string>
                {
                    {"Extension", req.Extension},
                    {"ContentType", req.ContentType}
                }
            }, cancellationToken);
        }

        var result = await _storageService.PutAsync(new PutObject
        {
            Name = req.Name,
            ContentType = req.ContentType,
            Path = req.FilePath,
            BucketName = "default",
        });

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

    public async Task GetAsync(GetFileRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}