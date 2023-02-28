using Core.ObjectStorages.Types;

namespace Core.ObjectStorages;

public class ObjectStorage
{
    public string Name { get; set; } = default!;

    public ObjectStorageStatus Status { get; set; } = ObjectStorageStatus.Enable;

    public Dictionary<string, string> Metas { get; set; } = new();
}