using Minio;
using Storages.Abstractions;

namespace Storages.MinIO;

internal sealed class MinIOStorageService : IStorageService
{
    private readonly MinioClient _client;
    private const int LinkExpireTimeInSeconds = 518400;

    public MinIOStorageService(MinioClient client)
    {
        _client = client;
    }

    public string Name => "minio";

    public async Task<FileLink> PutAsync(Stream stream, string path, string name)
    {
        var (bucketName, filePath) = ExtractPathData(path);
        var objectName = $"{filePath}/{name}";
        if (!await FileExitsAsync())
        {
            await _client.MakeBucketAsync(new MakeBucketArgs()
                .WithBucket(bucketName));
        }

        await _client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length));

        var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithExpiry(LinkExpireTimeInSeconds));

        return new FileLink
        {
            Url = url,
            ExpireDateTimeUtc = DateTime.UtcNow.AddSeconds(LinkExpireTimeInSeconds)
        };

        async Task<bool> FileExitsAsync()
        {
            return await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        }
    }


    public Task GetAsync(string path, string name, Action<Stream> outStream)
    {
        return _client.GetObjectAsync(new GetObjectArgs()
            .WithBucket(path)
            .WithObject(name)
            .WithCallbackStream(outStream));
    }

    public async Task<FileLink> RefreshLinkAsync(string path, string name)
    {
        var (bucketName, filePath) = ExtractPathData(path);
        var objectName = $"{filePath}/{name}";
        var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithExpiry(LinkExpireTimeInSeconds));

        return new FileLink
        {
            Url = url,
            ExpireDateTimeUtc = DateTime.UtcNow.AddSeconds(LinkExpireTimeInSeconds)
        };
    }

    public async Task DeleteAsync(string path, string name)
    {
        var (bucketName, filePath) = ExtractPathData(path);
        var objectName = $"{filePath}/{name}";
        await _client.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName));
    }


    private static (string, string) ExtractPathData(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return (string.Empty, string.Empty);
        }

        var pathArray = SplitPath(filePath);
        if (pathArray.Length == 0) return (string.Empty, string.Empty);
        var bucketName = pathArray.First();
        var finalPath = string.Join("/", pathArray[1..]);
        return (bucketName, finalPath);

        string[] SplitPath(string path)
        {
            var splitPath = path.Split("/");
            var finalSplit = splitPath.Length > 1 ? splitPath : path.Split("\\");
            return finalSplit.Where(s => s.Length > 0).ToArray();
        }
    }
}