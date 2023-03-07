using Minio;
using Providers.Abstractions;

namespace MinIO;

public class StorageService : IStorageService
{
    private readonly MinioClient _client;

    public StorageService(MinioClient client)
    {
        _client = client;
    }

    public string ProviderName => "minio";

    private const string BucketName = "default";

    private const int LinkExpireTimeInSeconds = 604800;


    public async Task<string> PutAsync(PutObject obj)
    {
        if (!await FileExitsAsync())
        {
            await _client.MakeBucketAsync(new MakeBucketArgs()
                .WithBucket(BucketName));
        }

        await _client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(obj.Name)
            .WithFileName(obj.Path)
            .WithContentType(obj.ContentType));

        var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(BucketName)
            .WithObject(obj.Name)
            .WithExpiry(LinkExpireTimeInSeconds));

        return url;

        async Task<bool> FileExitsAsync()
        {
            return await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(BucketName));
        }
    }


    public async Task<MemoryStream> GetAsync(GetObject obj)
    {
        await using var fileStream = new MemoryStream();

        var statObjectArgs = new StatObjectArgs()
            .WithBucket(BucketName)
            .WithObject(obj.Name);
        await _client.StatObjectAsync(statObjectArgs);

        var getObjectArgs = new GetObjectArgs()
            .WithBucket(BucketName)
            .WithObject(obj.Name)
            .WithCallbackStream(stream => { stream.CopyTo(fileStream); });
        await _client.GetObjectAsync(getObjectArgs);
        return fileStream;
    }

    public Task DeleteAsync(DeleteObject obj)
    {
        throw new NotImplementedException();
    }
}