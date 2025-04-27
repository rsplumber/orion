using Minio;
using Minio.DataModel.Args;
using Storages.Abstractions;

namespace Storages.MinIO;

internal sealed class MinIoStorageService : IStorageService
{
    private readonly IMinioClient _client;
    private const int LinkExpireTimeInSeconds = 3600;

    public MinIoStorageService(IMinioClient client)
    {
        _client = client;
    }

    public string Name => "minio";

    public async ValueTask<FileLink> PutAsync(Stream stream, string path, string name)
    {
        var (bucketName, filePath) = ExtractPathData(path);
        await EnsureBucketExistsAsync(bucketName).ConfigureAwait(false);

        var objectName = BuildObjectName(filePath, name);

        await _client.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length))
            .ConfigureAwait(false);

        var url = await GeneratePresignedUrlAsync(bucketName, objectName).ConfigureAwait(false);

        var fileLink = FileLinkArrayPool.Rent();
        fileLink.Url = url;
        fileLink.ExpireDateTimeUtc = DateTime.UtcNow.AddSeconds(LinkExpireTimeInSeconds);
        return fileLink;
    }

    public ValueTask GetAsync(string path, string name, Action<Stream> outStream)
    {
        var task = _client.GetObjectAsync(new GetObjectArgs()
            .WithBucket(path)
            .WithObject(name)
            .WithCallbackStream(outStream));

        return new ValueTask(task);
    }

    public async ValueTask<FileLink> RefreshLinkAsync(string path, string name)
    {
        var (bucketName, filePath) = ExtractPathData(path);
        var objectName = BuildObjectName(filePath, name);

        var url = await GeneratePresignedUrlAsync(bucketName, objectName).ConfigureAwait(false);

        var fileLink = FileLinkArrayPool.Rent();
        fileLink.Url = url;
        fileLink.ExpireDateTimeUtc = DateTime.UtcNow.AddSeconds(LinkExpireTimeInSeconds);
        return fileLink;
    }

    public ValueTask DeleteAsync(string path, string name)
    {
        var (bucketName, filePath) = ExtractPathData(path);
        var objectName = BuildObjectName(filePath, name);

        var task = _client.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName));

        return new ValueTask(task);
    }

    private async ValueTask EnsureBucketExistsAsync(string bucketName)
    {
        if (!await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)).ConfigureAwait(false))
        {
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName)).ConfigureAwait(false);
        }
    }

    private async ValueTask<string> GeneratePresignedUrlAsync(string bucketName, string objectName)
    {
        return await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry(LinkExpireTimeInSeconds))
            .ConfigureAwait(false);
    }

    private static string BuildObjectName(string path, string name) =>
        string.IsNullOrEmpty(path) ? name : $"{path}/{name}";

    private static (string BucketName, string FilePath) ExtractPathData(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return (string.Empty, string.Empty);

        ReadOnlySpan<char> pathSpan = filePath.AsSpan();
        var separatorIndex = pathSpan.IndexOfAny('/', '\\');

        if (separatorIndex == -1)
            return (filePath.Trim(), string.Empty);

        var bucketName = pathSpan[..separatorIndex].Trim().ToString();
        var remainingPath = pathSpan[(separatorIndex + 1)..].Trim().ToString();

        return (bucketName, remainingPath);
    }
}