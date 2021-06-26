using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EHunter.EHentai.Api.Models;
using EHunter.Media;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.EHentai.ViewModels
{
    internal class ThumbnailSource : CachedImageSource
    {
        private readonly Gallery _gallery;

        public ThumbnailSource(Gallery gallery, IMemoryCache memoryCache)
            : base(memoryCache) => _gallery = gallery;

        protected override object CreateCacheKey() => _gallery.Thumbnail;

        protected override Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
            => _gallery.RequestThumbnailAsync();
    }
}
