using Minio;
using Providers.Abstractions;

namespace MinIO;

public class StorageService : IStorageService
{
    public string ProviderName => "minio";  

    public string AccessKey => "EvZWw7VSbGCm4M9D";

    public string SecretKey => "5RQNFh5SI2NVxWiAxVp9VK5tvmWQ7BMN";

    public string Endpoint => "localhost:9000";

    public async Task<string> PutAsync(PutObject obj)
    {
        var minio = new MinioClient()
            .WithEndpoint(Endpoint)
            .WithCredentials(AccessKey, SecretKey)
            .Build();


        var beArgs = new BucketExistsArgs()
            .WithBucket(obj.BucketName);
        var found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs()
                .WithBucket(obj.BucketName);
            await minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        }

        // Upload a file to bucket.
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(obj.BucketName)
            .WithObject(obj.Name)
            .WithFileName(obj.Path)
            .WithContentType(obj.ContentType);
        await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
        return null;
    }


    public Task<string> GetAsync(GetObject obj)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteAsync(DeleteObject obj)
    {
        throw new NotImplementedException();
    }
}