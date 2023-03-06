using Minio;
using Providers.Abstractions;

namespace MinIO;

public class StorageService : IStorageService
{
    public StorageService(MinioClient client)
    {
        _client = client
            .WithEndpoint(Endpoint)
            .WithCredentials(AccessKey, SecretKey)
            .Build();
        ;
    }

    private readonly MinioClient _client;
    
    public string ProviderName => "minio";

    private const string BucketName = "default";

    private const string AccessKey = "EvZWw7VSbGCm4M9D";

    private const string SecretKey = "5RQNFh5SI2NVxWiAxVp9VK5tvmWQ7BMN";

    private const string Endpoint = "localhost:9000";


    public async Task<string> PutAsync(PutObject obj)
    {


        var beArgs = new BucketExistsArgs()
            .WithBucket(BucketName);
        var found = await _client.BucketExistsAsync(beArgs).ConfigureAwait(false);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs()
                .WithBucket(BucketName);
            await _client.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        }

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(obj.Name)
            .WithFileName(obj.Path)
            .WithContentType(obj.ContentType);
        await _client.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

        return obj.Name;
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