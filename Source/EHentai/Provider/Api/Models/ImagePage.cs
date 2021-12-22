using System.Text.RegularExpressions;
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

        private static readonly Regex s_nlRegex = new(@"return nl\('(.*)'\)",
            RegexOptions.Compiled | RegexOptions.ECMAScript | RegexOptions.CultureInvariant);
        public async Task<ImagePageDetail> GetImageAsync(CancellationToken cancellationToken = default)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);

            using var request = await _client.HttpClient.GetStreamAsync(PageUri, cancellationToken).ConfigureAwait(false);
            var document = await context.OpenAsync(req => req.Content(request), cancellationToken).ConfigureAwait(false);

            var img = document.QuerySelector<IHtmlImageElement>("img#img");
            var original = document.QuerySelector<IHtmlAnchorElement>("div#i7>a");
            var loadFail = document.QuerySelector<IHtmlAnchorElement>("a#loadfail");

            return new(
                new(img!.Source!),
                original is null ? null : new(original.Href),
                s_nlRegex.Match(loadFail!.GetAttribute("onclick")!).Groups[1].Value);
        }
    }

    public record struct ImagePageDetail(
        Uri ImageUri,
        Uri? Original,
        string RefreshKey);
}
