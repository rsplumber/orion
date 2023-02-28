using Core.Files.Events;
using DotNetCore.CAP;

namespace Core.Files.Services;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly ICapPublisher _capPublisher;


    public FileService(IFileRepository fileRepository, ICapPublisher capPublisher)
    {
        _fileRepository = fileRepository;
        _capPublisher = capPublisher;
    }

    public async Task PutAsync(PutFileRequest req, CancellationToken cancellationToken)
    {


        await using (var fs = System.IO.File.Open(req.FilePath, FileMode.Open))
        {
            await _fileRepository.AddAsync(new File
            {
                Name = fs.Name,
                Metas = new Dictionary<string, string>
                {
                    {"Extension", req.Extension},
                    {"Mime", req.Mime}
                }
            }, cancellationToken);
        }
        
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