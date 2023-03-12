namespace Queries.Providers;

public class ProviderResponse
{
    public string Name { get; set; }

    public string Status { get; set; }

    public Dictionary<string, string> Metas { get; set; }
}