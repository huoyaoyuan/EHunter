namespace EHunter.Media
{
    public interface IImageSource
    {
        ValueTask<Stream> GetImageAsync(bool refresh = false, CancellationToken cancellationToken = default);
    }
}
