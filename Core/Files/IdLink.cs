using CSharpVitamins;

namespace Core.Files;

public static class IdLink
{
    public static Guid Parse(string link)
    {
        ShortGuid extractedLink = link;
        return extractedLink.Guid;
    }

    public static string From(Guid id)
    {
        ShortGuid extractedLink = id;
        return extractedLink.Value;
    }
}