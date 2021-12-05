using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EHunter.EHentai.Api.Json;

namespace EHunter.EHentai.Api.Models
{
    public class Gallery
    {
        private readonly EHentaiClient _client;
        private static readonly Regex s_titleRegex
            = new(@"^(?:\((.+?)\))?\s*(?:\[(.+?)\s*\((.+?)\)\])?\s*(.+?)\s*(?:\((.+?)\))?(?:\s*[\[【](.+?)[\]】])*$",
                RegexOptions.Compiled | RegexOptions.ECMAScript | RegexOptions.CultureInvariant);

        // TODO: use css parser when AngleSharp.Css is working
        private static readonly Regex s_pageThumbnailRegex
            = new(@"background:transparent url\((.+?)\)",
                RegexOptions.Compiled | RegexOptions.ECMAScript | RegexOptions.CultureInvariant);

        internal Gallery(EHentaiClient client, Uri queryUri, GalleryMetadata metadata)
        {
            _client = client;
            GalleryUri = new Uri($"https://{queryUri.Host}/g/{metadata.Gid}/{metadata.Token}");

            Id = metadata.Gid;
            Token = metadata.Token;
            Category = metadata.Category;
            Thumbnail = metadata.Thumb;
            Posted = metadata.Posted;
            Tags = metadata.Tags.ToArray();
            Title = ParseTitle(metadata.Title);
            TitleJpn = ParseTitle(metadata.TitleJpn);

            static string? NullIfEmpty(string str)
                => string.IsNullOrEmpty(str) ? null : str;

            static ParsedTitle? ParseTitle(string? title)
            {
                if (string.IsNullOrEmpty(title))
                    return null;

                var match = s_titleRegex.Match(title);
                return new ParsedTitle(
                    Original: title,
                    Market: NullIfEmpty(match.Groups[1].Value),
                    Group: NullIfEmpty(match.Groups[2].Value),
                    Artist: NullIfEmpty(match.Groups[3].Value),
                    TitleBody: NullIfEmpty(match.Groups[4].Value),
                    Parody: NullIfEmpty(match.Groups[5].Value),
                    Properties: match.Groups[6].Captures
                        .Select(x => x.Value)
                        .ToImmutableArray());
            }
        }

        public int Id { get; }
        public string Token { get; }
        public Uri GalleryUri { get; }

        public GalleryCategory Category { get; }
        public Uri Thumbnail { get; }
        public DateTimeOffset Posted { get; }
        public IReadOnlyList<Tag> Tags { get; }
        public ParsedTitle? Title { get; }
        public ParsedTitle? TitleJpn { get; }

        public Task<Stream> RequestThumbnailAsync() => _client.HttpClient.GetStreamAsync(Thumbnail);

        public async IAsyncEnumerable<GalleryPage> GetPagesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            int totalWebPages = 0;
            int p = 0;

            var config = Configuration.Default;
            var context = BrowsingContext.New(config);

            do
            {
                using var request = await _client.HttpClient.GetStreamAsync($"{GalleryUri}?p={p}", cancellationToken).ConfigureAwait(false);
                var document = await context.OpenAsync(req => req.Content(request), cancellationToken).ConfigureAwait(false);

                totalWebPages = int.Parse(
                    document
                        .QuerySelector("table.ptt")!
                        .QuerySelectorAll("td")[^2]
                        .QuerySelector("a")!
                        .Text(),
                    null);

                foreach (var pageDiv in document
                    .QuerySelector("div#gdt")!
                    .QuerySelectorAll<IHtmlDivElement>("div.gdtm>div"))
                {
                    string url = pageDiv.QuerySelector<IHtmlAnchorElement>("a")!.Href;
                    string style = pageDiv.GetAttribute("style")!;
                    var match = s_pageThumbnailRegex.Match(style);
                    string thumbnail = match.Groups[1].Value;

                    yield return new(_client, new Uri(url), new Url(thumbnail));
                }
            }
            while (p < totalWebPages);
        }
    }

    public record ParsedTitle(
        string Original,
        string? Market,
        string? Group,
        string? Artist,
        string? TitleBody,
        string? Parody,
        ImmutableArray<string> Properties);

    [JsonConverter(typeof(TagConverter))]
    public record struct Tag(string? Namespace, string Name);
}
