using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EHunter.EHentai.Api.Json;

namespace EHunter.EHentai.Api.Models
{
    public class Gallery
    {
        private readonly EHentaiClient _client;
        private static readonly Regex s_titleRegex
            = new(@"^(?:\((.+?)\))?\s*(?:\[(.+?)\s*\((.+?)\)\])?\s*(.+?)\s*(?:\((.+?)\))?(?:\s*[\[【](.+?)[\]】])*$",
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

            static ParsedTitle ParseTitle(string title)
            {
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
        public IReadOnlyList<string> Tags { get; }
        public ParsedTitle Title { get; }
        public ParsedTitle TitleJpn { get; }

        public record ParsedTitle(
            string Original,
            string? Market,
            string? Group,
            string? Artist,
            string? TitleBody,
            string? Parody,
            ImmutableArray<string> Properties);

        public Task<byte[]> GetImageAsync() => _client.HttpClient.GetByteArrayAsync(Thumbnail);
    }
}
