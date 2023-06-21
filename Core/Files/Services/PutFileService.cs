using Core.Providers;
using Core.Replications.Events;
using Core.Storages;
using DotNetCore.CAP;

namespace Core.Files.Services;

internal sealed class PutFileService : IPutFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IProviderRepository _providerRepository;
    private readonly IStorageService _storageService;
    private readonly ICapPublisher _capPublisher;

    public PutFileService(IFileRepository fileRepository, IProviderRepository providerRepository, IStorageService storageService, ICapPublisher capPublisher)
    {
        _fileRepository = fileRepository;
        _providerRepository = providerRepository;
        _storageService = storageService;
        _capPublisher = capPublisher;
    }

    public async Task<PutFileResponse> PutAsync(Stream stream, PutFileRequest req, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var (bucketName, filePath) = ExtractPathData(req.OwnerId, req.FilePath);
        var fileName = $"{filePath}{id}{req.Extension}";
        var link = await _storageService.PutAsync(stream, fileName, bucketName);
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

        return new PutFileResponse(file.Id, IdLink.From(file.Id));
    }

    private static (string, string) ExtractPathData(Guid ownerId, string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return (string.Empty, string.Empty);
        }

        var pathArray = SplitPath(filePath);
        var bucketName = ownerId.ToString();
        var finalPath = $"{string.Join("/", pathArray[Range.StartAt(0)])}/";
        return (bucketName, finalPath);

        string[] SplitPath(string path)
        {
            var splitPath = path.Split("/");
            var finalSplit = splitPath.Length > 1 ? splitPath : path.Split("\\");
            return finalSplit.Where(s => s.Length > 0).ToArray();
        }
    }
}