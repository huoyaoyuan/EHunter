using System.Threading.Tasks;

#nullable enable

namespace EHunter.Provider.Pixiv.Services.ImageCaching
{
    public abstract class ImageRequest
    {
        public abstract object? MemoryCacheKey { get; }

        public abstract Task<byte[]> GetImageAsync();
    }
}
