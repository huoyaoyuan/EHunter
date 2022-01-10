using EHunter.EHentai.Api;
using EHunter.EHentai.Api.Models;
using EHunter.Media;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.EHentai.Media
{
    internal class ImagePreviewSource : CachedImageSource
    {
        private readonly EHentaiClient _client;
        private readonly ImagePage _image;

        public ImagePreviewSource(
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
        protected override Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
            => _client.HttpClient.GetStreamAsync(_image.Thumbnail, cancellationToken);
    }
}
