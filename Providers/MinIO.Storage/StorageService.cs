using Core;
using Minio;

namespace MinIO;

public class StorageService : IStorageService
{
    private readonly MinioClient _client;

    public StorageService(MinioClient client)
    {
        _client = client;
    }

    public string Provider => "minio";

    private const int LinkExpireTimeInSeconds = 518400;


    public async Task<Link> PutAsync(Stream stream, PutObject obj)
    {
        if (!await FileExitsAsync())
        {
            await _client.MakeBucketAsync(new MakeBucketArgs()
                .WithBucket(obj.BucketName));
        }

        await _client.PutObjectAsync(new PutObjectArgs()
                .WithBucket(obj.BucketName)
                .WithObject(obj.Name)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length))
            ;
        var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(obj.BucketName)
            .WithObject(obj.Name)
            .WithExpiry(LinkExpireTimeInSeconds));

        return new Link()
        {
            Url = url,
            ExpireDateTimeUtc = DateTime.UtcNow.AddSeconds(LinkExpireTimeInSeconds)
        };

        async Task<bool> FileExitsAsync()
        {
            return await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(obj.BucketName));
        }
    }


    public Task GetAsync(string path, string name, Action<Stream> outStream)
    {
        return _client.GetObjectAsync(new GetObjectArgs()
            .WithBucket(path)
            .WithObject(name)
            .WithCallbackStream(outStream));
    }

    public async Task<Link> RefreshLinkAsync(string path, string name)
    {
        var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(path)
            .WithObject(name)
            .WithExpiry(LinkExpireTimeInSeconds));

        return new Link()
        {
            Url = url,
            ExpireDateTimeUtc = DateTime.UtcNow.AddSeconds(LinkExpireTimeInSeconds)
        };
    }

    public async Task DeleteAsync(string path, string name)
    {
        var bucketName = path.Split("/").First();
        var correctPath = string.Join("/", path.Split("/")[1..]) + name;
        await _client.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(correctPath));
    }
}