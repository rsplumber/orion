using Minio;

namespace MinIO.Sample;

public class CloudMinioClient
{
    public CloudMinioClient(MinioClient client)
    {
        Client = client;
    }

    public MinioClient Client { get; set; }
}