namespace Core.Files.Services;

internal sealed class LocationSelector : ILocationSelector
{
    public Task<FileLocation?> SelectAsync(List<FileLocation> locations, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(locations.FirstOrDefault());
    }
}