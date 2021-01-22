using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Services.ImageCaching
{
    public class ImageCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public ImageCacheService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public async Task<ImageEntry> GetImageAsync(ImageRequest request, bool clearMemoryCache)
        {
            object? key = request.MemoryCacheKey;
            if (key is null)
                return new ImageEntry(await request.GetImageAsync().ConfigureAwait(false));

            if (clearMemoryCache)
                _memoryCache.Remove(key);

            return await _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                byte[] data = await request.GetImageAsync().ConfigureAwait(false);
                entry.SetSize(data.Length);
                return new ImageEntry(data);
            }).ConfigureAwait(false);
        }
    }
}
