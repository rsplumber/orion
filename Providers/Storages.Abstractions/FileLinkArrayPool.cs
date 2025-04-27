namespace Storages.Abstractions;

public static class FileLinkArrayPool
{
    private const int MaxPoolSize = 1000;
    private static readonly FileLink[] Pool = new FileLink[MaxPoolSize];
    private static int _freeIndex = 0;

    static FileLinkArrayPool()
    {
        for (var i = 0; i < MaxPoolSize; i++)
        {
            Pool[i] = new FileLink();
        }
    }

    public static FileLink Rent()
    {
        var index = Interlocked.Decrement(ref _freeIndex);
        if (index >= 0)
        {
            var link = Pool[index];
            link.SetIndex(index);
            return link;
        }

        // Pool exhausted, create new without tracking
        var newLink = new FileLink();
        newLink.SetIndex(-1);
        return newLink;
    }

    public static void Return(FileLink fileLink, int index)
    {
        fileLink.Url = string.Empty;
        fileLink.ExpireDateTimeUtc = default;
        if (index is >= 0 and < MaxPoolSize)
        {
            Pool[index] = fileLink;
            Interlocked.Increment(ref _freeIndex);
        }
    }
}