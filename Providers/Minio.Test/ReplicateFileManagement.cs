using Core;
using Core.Files;
using Core.Replications;
using DotNetCore.CAP;

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
                .WithBucket(pp), cancellationToken);
        }

        using var memory = new MemoryStream();
        await _storageService.GetAsync(file!.Path, file.Name, async stream =>
        {
            await stream.CopyToAsync(memory, cancellationToken);
            memory.Seek(0, SeekOrigin.Begin);
        });

        await _client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(pp)
            .WithObject(file.Name)
            .WithStreamData(memory)
            .WithObjectSize(memory.Length), cancellationToken);

        var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(pp)
            .WithObject(file.Name)
            .WithExpiry(546506));


        async Task<bool> FileExitsAsync()
        {
            return await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(pp), cancellationToken);
        }

        return false;
    }

    protected override async Task<bool> DeleteFileAsync(Guid filedId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}