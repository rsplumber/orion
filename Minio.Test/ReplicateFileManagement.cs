using Core.Replications;
using DotNetCore.CAP;

namespace Minio.Test;

internal sealed class ReplicateFileManagement : AbstractReplicationManagement
{
    public ReplicateFileManagement(ICapPublisher capPublisher, IReplicationRepository replicationRepository)
        : base(capPublisher, replicationRepository)
    {
    }

    public override string Provider => "minio_test";

    protected override int MaximumRetryCount => 2;

    protected override async Task<bool> ReplicateFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        return false;
    }
}