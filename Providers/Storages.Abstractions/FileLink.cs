namespace Storages.Abstractions;

public sealed class FileLink : IDisposable
{
    private int _index = -1;

    internal void SetIndex(int index)
    {
        _index = index;
    }

    public string Url { get; set; } = string.Empty;
    public DateTime ExpireDateTimeUtc { get; set; }

    public void Dispose()
    {
        if (_index < 0) return;
        FileLinkArrayPool.Return(this, _index);
        _index = -1;
    }
}