using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Media;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Services.ImageCaching
{
    public abstract class CachedImageSource : IImageSource
    {
        private readonly IMemoryCache _memoryCache;
        private byte[]? _data;

        protected CachedImageSource(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public async ValueTask<Stream> GetImageAsync(bool refresh = false, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!refresh)
            {
                if (_data is not null)
                    return new MemoryStream(_data);
            }

            object cacheKey = CreateCacheKey();
            if (refresh)
            {
                _data = null;
                _memoryCache.Remove(cacheKey);
            }

            _data = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                var stream = await RequestAsync(cancellationToken).ConfigureAwait(false);
                using var mms = new MemoryStream();
                await stream.CopyToAsync(mms).ConfigureAwait(false);
                entry.SetSize(mms.Length);
                return mms.ToArray();
            }).ConfigureAwait(false);

            return new MemoryStream(_data);
        }

        protected abstract object CreateCacheKey();

        protected abstract Task<Stream> RequestAsync(CancellationToken cancellationToken = default);
    }
}
