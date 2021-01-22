using System.Threading.Tasks;

namespace EHunter.Services.ImageCaching
{
    public abstract class ImageRequest
    {
        public abstract object? MemoryCacheKey { get; }

        public abstract Task<byte[]> GetImageAsync();
    }
}
