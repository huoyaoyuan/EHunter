using EHunter.EHentai.Api;
using EHunter.EHentai.Api.Models;
using EHunter.Media;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.EHentai.Media
{
    internal class ThumbnailSource : CachedImageSource
    {
        private readonly EHentaiClient _client;
        private readonly Gallery _gallery;

        public ThumbnailSource(EHentaiClient client, Gallery gallery, IMemoryCache memoryCache)
            : base(memoryCache)
        {
            _client = client;
            _gallery = gallery;
        }

        private record CacheKey(int Id);

        protected override object CreateCacheKey() => new CacheKey(_gallery.Id);

        protected override Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
            => _client.HttpClient.GetStreamAsync(_gallery.Thumbnail, cancellationToken);
    }
}
