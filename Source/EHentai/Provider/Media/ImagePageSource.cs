using EHunter.EHentai.Api;
using EHunter.EHentai.Api.Models;
using EHunter.Media;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.EHentai.Media
{
    internal class ImagePageSource : CachedImageSource
    {
        private readonly EHentaiClient _client;
        private readonly ImagePage _image;

        public ImagePageSource(
            EHentaiClient client,
            ImagePage image,
            IMemoryCache memoryCache)
            : base(memoryCache)
        {
            _client = client;
            _image = image;
        }

        private record CacheKey(Uri PageUri);

        protected override object CreateCacheKey() => _image.PageUri;
        protected override async Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
        {
            var detail = await _image.GetImageAsync(cancellationToken).ConfigureAwait(false);
            return await _client.HttpClient.GetStreamAsync(detail.ImageUri, cancellationToken).ConfigureAwait(false);
        }
    }
}
