using Minio;
using Providers.Abstractions;

namespace MinIO;

public class StorageService : IStorageService
{
    public string ProviderName => "minio";

    private static string BucketName => "default";

    private static string AccessKey => "EvZWw7VSbGCm4M9D";

    private static string SecretKey => "5RQNFh5SI2NVxWiAxVp9VK5tvmWQ7BMN";

    private static string Endpoint => "localhost:9000";

    public async Task<string> PutAsync(PutObject obj)
    {
        var minio = new MinioClient()
            .WithEndpoint(Endpoint)
            .WithCredentials(AccessKey, SecretKey)
            .Build();


        var beArgs = new BucketExistsArgs()
            .WithBucket(BucketName);
        var found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs()
                .WithBucket(BucketName);
            await minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        }

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(obj.Name)
            .WithFileName(obj.Path)
            .WithContentType(obj.ContentType);
        await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
        return obj.Name;
    }


    public async Task<MemoryStream> GetAsync(GetObject obj)
    {
        await using var fileStream = new MemoryStream();
        
        var minio = new MinioClient()
            .WithEndpoint(Endpoint)
            .WithCredentials(AccessKey, SecretKey)
            .Build();

        var statObjectArgs = new StatObjectArgs()
            .WithBucket(BucketName)
            .WithObject(obj.Name);
        await minio.StatObjectAsync(statObjectArgs);

        var getObjectArgs = new GetObjectArgs()
            .WithBucket(BucketName)
            .WithObject(obj.Name)
            .WithCallbackStream(stream => { stream.CopyTo(fileStream); });
        await minio.GetObjectAsync(getObjectArgs);
        return fileStream;
    }

    public Task<string> DeleteAsync(DeleteObject obj)
    {
        throw new NotImplementedException();
    }
}