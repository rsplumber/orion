using Core.Files.Exceptions;
using Core.Providers;
using Core.Replications.Events;
using CSharpVitamins;
using DotNetCore.CAP;

namespace Core.Files.Services;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly IStorageService _storageService;
    private readonly ICapPublisher _capPublisher;


    public FileService(IFileRepository fileRepository, ICapPublisher capPublisher, IStorageService storageService, IProviderRepository providerRepository)
    {
        _fileRepository = fileRepository;
        _capPublisher = capPublisher;
        _storageService = storageService;
        _providerRepository = providerRepository;
    }

    public async Task<PutFileResponse> PutAsync(Stream stream, PutFileRequest req, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var (bucketName, filePath) = ExtractPathData(req);

        var link = await _storageService.PutAsync(stream, new PutObject
        {
            Length = req.Lenght,
            Name = filePath + id + req.Extension,
            BucketName = bucketName
        });

        var file = new File
        {
            Id = id,
            Name = id + req.Extension,
            Path = bucketName + "/" + filePath,
            Metas = new Dictionary<string, string>
            {
                { "Extension", req.Extension }
            }
        };

        file.Add(new FileLocation
        {
            Link = link.Url,
            ExpireDateUtc = link.ExpireDateTimeUtc,
            Provider = _storageService.Provider,
        });

        await _fileRepository.AddAsync(file, cancellationToken);

        foreach (var provider in await _providerRepository.FindAsync(cancellationToken))
        {
            await Task.Run(() =>
            {
                _capPublisher.PublishAsync($"{ReplicateFileEvent.EventName}.{provider.Name}", new ReplicateFileEvent
                {
                    FileId = file.Id,
                    Provider = provider.Name
                }, cancellationToken: cancellationToken);
            }, cancellationToken);
        }

        return new PutFileResponse(file.Id, GenerateLink(file.Id));
    }


    public async Task DeleteAsync(DeleteFileRequest req, CancellationToken cancellationToken = default)
    {
        var fileId = ExtractId(req.Link);
        var file = await _fileRepository.FindAsync(fileId, cancellationToken);
        await _storageService.DeleteAsync(file!.Path, file.Name);
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

        var fileLocation = file.Locations.First(location => location.Provider == "minio");
        if (fileLocation.ExpireDateUtc > DateTime.UtcNow) return fileLocation.Link;
        var link = await _storageService.RefreshLinkAsync(file.Path, file.Name);
        fileLocation.Link = link.Url;
        fileLocation.ExpireDateUtc = link.ExpireDateTimeUtc;
        await _fileRepository.UpdateAsync(file, cancellationToken);
        return fileLocation.Link;
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


    private static (string, string) ExtractPathData(PutFileRequest req)
    {
        if (string.IsNullOrEmpty(req.FilePath))
        {
            return (string.Empty, string.Empty);
        }

        var pathArray = SplitPath(req.FilePath);
        var bucketName = req.OwnerId.ToString();
        var filePath = $"{string.Join("/", pathArray[Range.StartAt(0)])}/";
        return (bucketName, filePath);

        string[] SplitPath(string path)
        {
            var splitPath = path.Split("/");
            var finalSplit = splitPath.Length > 1 ? splitPath : path.Split("\\");
            return finalSplit.Where(s => s.Length > 0).ToArray();
        }
    }
}