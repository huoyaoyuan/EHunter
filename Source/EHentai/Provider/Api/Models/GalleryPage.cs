namespace EHunter.EHentai.Api.Models
{
    public class GalleryPage
    {
        private readonly EHentaiClient _client;

        internal GalleryPage(EHentaiClient client, Uri pageUri, Uri thumbnail)
        {
            _client = client;
            PageUri = pageUri;
            Thumbnail = thumbnail;
        }

        public Uri PageUri { get; }
        public Uri Thumbnail { get; }

        public Task<Stream> RequestThumbnailAsync() => _client.HttpClient.GetStreamAsync(Thumbnail);
    }
}
