using Core.Files.Exceptions;
using CSharpVitamins;

namespace Core.Files;

public static class IdLink
{
    public static Guid Parse(string link)
    {
        try
        {
            ShortGuid extractedLink = link;
            return extractedLink.Guid;
        }
        catch
        {
            throw new InvalidLinkException();
        }
    }

    public static string From(Guid id)
    {
        ShortGuid extractedLink = id;
        return extractedLink.Value;
    }
}