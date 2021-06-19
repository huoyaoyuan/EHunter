using System;
using System.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Media;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.Services.ImageCaching
{
    [Export]
    public class PixivImageService
    {
        private readonly IMemoryCache _memoryCache;

        [ImportingConstructor]
        public PixivImageService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public IImageSource GetImage(ImageInfo imageInfo) => new PixivImageSource(this, imageInfo);

        private class PixivImageSource : IImageSource
        {
            private readonly PixivImageService _owner;
            private readonly ImageInfo _imageInfo;
            private byte[]? _data;

            public PixivImageSource(PixivImageService owner, ImageInfo imageInfo)
            {
                _owner = owner;
                _imageInfo = imageInfo;
            }

            public ValueTask<Stream> GetImageAsync(bool refresh = false, CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!refresh && _data != null)
                    return new(new MemoryStream(_data));

                var cacheKey = new PixivCacheKey(_imageInfo.Uri);
                if (!refresh)
                {
                    if (_owner._memoryCache.TryGetValue(cacheKey, out _data))
                        return new(new MemoryStream(_data!));
                }
                else
                {
                    _data = null;
                    _owner._memoryCache.Remove(cacheKey);
                }

                return GetAsync(cacheKey, cancellationToken);

                async ValueTask<Stream> GetAsync(PixivCacheKey cacheKey, CancellationToken cancellationToken)
                {
                    var response = await _imageInfo.RequestAsync(cancellationToken).ConfigureAwait(false);
                    _data = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                    _owner._memoryCache.Set(cacheKey, _data);
                    return new MemoryStream(_data);
                }
            }
        }
    }

    internal record PixivCacheKey(Uri Uri);
}
