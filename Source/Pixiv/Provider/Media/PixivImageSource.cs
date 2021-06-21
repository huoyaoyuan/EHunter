using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Media;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.Media
{
    internal class PixivImageSource : CachedImageSource
    {
        internal ImageInfo ImageInfo { get; }

        public PixivImageSource(ImageInfo imageInfo, IMemoryCache memoryCache)
            : base(memoryCache) => ImageInfo = imageInfo;

        private record CacheKey(Uri Uri);

        protected override object CreateCacheKey() => new CacheKey(ImageInfo.Uri);

        protected override Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
            => ImageInfo.RequestStreamAsync(cancellationToken);
    }
}
