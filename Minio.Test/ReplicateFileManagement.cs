using Core.Files;
using Core.Replications;
using DotNetCore.CAP;
using Providers.Abstractions;

namespace Minio.Test;

internal sealed class ReplicateFileManagement : AbstractReplicationManagement
{
    private readonly IFileRepository _fileRepository;
    private readonly IStorageService _storageService;
    private readonly MinioClient _client;

    public ReplicateFileManagement(ICapPublisher capPublisher, IReplicationRepository replicationRepository, IStorageService storageService, IFileRepository fileRepository, MinioClient client)
        : base(capPublisher, replicationRepository)
    {
        _storageService = storageService;
        _fileRepository = fileRepository;
        _client = client;
    }

    public override string Provider => "minio_test";

    protected override int MaximumRetryCount => 2;

    protected override async Task<bool> ReplicateFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        var file = await _fileRepository.FindAsync(fileId, cancellationToken);
        var pp = "klfsk";
        if (!await FileExitsAsync())
        {
            await _client.MakeBucketAsync(new MakeBucketArgs()
                .WithBucket(pp));
        }
        
        using (var fs = _storageService.GetAsync(file!.Path, file.Name))
        {
            await _client.PutObjectAsync(new PutObjectArgs()
                .WithBucket(pp)
                .WithObject(file.Name)
                .WithStreamData(fs)
                .WithObjectSize(fs.Length), cancellationToken);
        }

        var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(pp)
            .WithObject(pp)
            .WithExpiry(54656));


        async Task<bool> FileExitsAsync()
        {
            return await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(pp), cancellationToken);
        }

        return false;
    }
}