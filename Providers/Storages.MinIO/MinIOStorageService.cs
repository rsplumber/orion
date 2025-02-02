using Minio;
using Minio.DataModel.Args;
using Storages.Abstractions;

namespace Storages.MinIO;

internal sealed class MinIOStorageService : IStorageService
{
    private readonly IMinioClient _client;
    private const int LinkExpireTimeInSeconds = 60 * 60;

    public MinIOStorageService(IMinioClient client)
    {
        _client = client;
    }

    public string Name => "minio";

    public async Task<FileLink> PutAsync(Stream stream, string path, string name)
    {
        var (bucketName, filePath) = ExtractPathData(path);
        if (!await _client.BucketExistsAsync(new BucketExistsArgs()
                    .WithBucket(bucketName))
                .ConfigureAwait(false))
        {
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName)).ConfigureAwait(false);
        }

        var objectName = $"{filePath}/{name}";
        await _client.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length))
            .ConfigureAwait(false);

        var url = await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry(LinkExpireTimeInSeconds))
            .ConfigureAwait(false);

        return new FileLink
        {
            Url = url,
            ExpireDateTimeUtc = DateTime.UtcNow.AddSeconds(LinkExpireTimeInSeconds)
        };
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
                .WithExpiry(LinkExpireTimeInSeconds))
            .ConfigureAwait(false);

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
                .WithObject(objectName))
            .ConfigureAwait(false);
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