using EHunter.EHentai.Api;
using EHunter.EHentai.Api.Models;
using EHunter.Media;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.EHentai.ViewModels
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

        protected override object CreateCacheKey() => _gallery.Thumbnail;

        protected override Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
            => _client.HttpClient.GetStreamAsync(_gallery.Thumbnail, cancellationToken);
    }
}
