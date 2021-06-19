using System.Composition;
using System.IO;
using System.Threading.Tasks;
using EHunter.Media;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Services.ImageCaching
{
    [Export, Shared]
    public class ImageCacheService : IStorageService<ICachable>
    {
        private readonly IMemoryCache _memoryCache;

        [ImportingConstructor]
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

        public ValueTask<bool> IsStored(ICachable key) => new(_memoryCache.TryGetValue(key.GetCacheKey(), out _));

        public ValueTask<Stream?> TryGet(ICachable key)
        {
            if (_memoryCache.TryGetValue(key.GetCacheKey(), out byte[] value))
                return new(new MemoryStream(value));
            return default;
        }

        public async ValueTask Store(ICachable key, Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[stream.Length];
            await stream.CopyToAsync(new MemoryStream(data)).ConfigureAwait(false);
            _memoryCache.Set(key.GetCacheKey(), data);
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
