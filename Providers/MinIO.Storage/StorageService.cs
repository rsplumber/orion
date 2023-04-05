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


    private const int LinkExpireTimeInSeconds = 604800;


    public async Task<string> PutAsync(Stream stream, PutObject obj)
    {
        if (!await FileExitsAsync())
        {
            await _client.MakeBucketAsync(new MakeBucketArgs()
                .WithBucket(obj.Path));
        }

        await _client.PutObjectAsync(new PutObjectArgs()
                .WithBucket(obj.Path)
                .WithObject(obj.Name)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length))
            ;
        var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(obj.Path)
            .WithObject(obj.Name)
            .WithExpiry(LinkExpireTimeInSeconds));

        return url;

        async Task<bool> FileExitsAsync()
        {
            return await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(obj.Path));
        }
    }


    public Task GetAsync(string path, string name, Action<Stream> outStream)
    {
        return _client.GetObjectAsync(new GetObjectArgs()
            .WithBucket(path)
            .WithObject(name)
            .WithCallbackStream(outStream));
    }

    public Task DeleteAsync(string path, string name)
    {
        throw new NotImplementedException();
    }
}