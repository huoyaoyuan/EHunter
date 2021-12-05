using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace EHunter.EHentai.Api.Models
{
    public class ImagePage
    {
        private readonly EHentaiClient _client;

        internal ImagePage(EHentaiClient client, Uri pageUri, Uri thumbnail)
        {
            _client = client;
            PageUri = pageUri;
            Thumbnail = thumbnail;
        }

        public Uri PageUri { get; }
        public Uri Thumbnail { get; }

        public Task<Stream> RequestThumbnailAsync(CancellationToken cancellationToken = default) => _client.HttpClient.GetStreamAsync(Thumbnail, cancellationToken);

        public async Task<Stream> GetImageAsync(CancellationToken cancellationToken = default)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);

            using var request = await _client.HttpClient.GetStreamAsync(PageUri, cancellationToken).ConfigureAwait(false);
            var document = await context.OpenAsync(req => req.Content(request), cancellationToken).ConfigureAwait(false);

            var image = document
                .QuerySelector<IHtmlImageElement>("img#img");
            return await _client.HttpClient.GetStreamAsync(image!.Source, cancellationToken).ConfigureAwait(false);
        }
    }
}
