using System.Collections.Immutable;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EHunter.EHentai.Api.Json;
using EHunter.EHentai.Api.Models;

namespace EHunter.EHentai.Api
{
    public sealed class EHentaiClient : IDisposable
    {
        internal HttpClient HttpClient;
        private readonly CookieContainer _cookies = new();

        public EHentaiClient(IWebProxy? proxy = null)
            => HttpClient = CreateNewClient(proxy);

        public void SetProxy(IWebProxy? proxy)
        {
            HttpClient.Dispose();
            HttpClient = CreateNewClient(proxy);
        }

        private HttpClient CreateNewClient(IWebProxy? proxy)
        {
            return new HttpClient(
                new HttpClientHandler
                {
                    UseProxy = proxy is not null,
                    Proxy = proxy,
                    UseCookies = true,
                    CookieContainer = _cookies,
                    CheckCertificateRevocationList = true
                });
        }

        public void Dispose()
        {
            HttpClient.Dispose();
            ((IDisposable)_browsingContext).Dispose();
        }

        private readonly BrowsingContext _browsingContext = new(Configuration.Default);

        public bool UseExHentai { get; set; }

        public bool IsLogin { get; private set; }

        public async Task<IDocument> OpenDocumentAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            using var stream = await HttpClient.GetStreamAsync(uri, cancellationToken).ConfigureAwait(false);
            return await OpenDocumentAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        public Task<IDocument> OpenDocumentAsync(Stream stream, CancellationToken cancellationToken = default)
            => _browsingContext.OpenAsync(req => req.Content(stream), cancellationToken);

        public async Task<(string memberId, string passHash)> LoginAsync(string username, string password)
        {
            if (IsLogin)
                throw new InvalidOperationException("Logout first.");

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://forums.e-hentai.org/index.php?act=Login&CODE=01")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "UserName", username },
                    { "PassWord", password },
                    { "CookieDate", "1" },
                }!)
            };

            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);

            var cookies = _cookies.GetCookies(new Uri("https://e-hentai.org"));
            string? memberId = cookies[MemberIdCookie]?.Value, passHash = cookies[PassHashCookie]?.Value;

            if (memberId is null || passHash is null)
            {
                // try to identify the failure reason
                var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var document = await OpenDocumentAsync(responseStream).ConfigureAwait(false);
                var title = document.QuerySelectorAll<IHtmlDivElement>("div.formsubtitle")
                    .FirstOrDefault(x => x.Text() == "The following errors were found:");
                if (title != null)
                {
                    var span = title.ParentElement?.QuerySelector("span.postcolor");
                    if (span != null)
                        throw new InvalidOperationException(span.Text());
                }

                throw new InvalidOperationException("Unknown error happed while login.");
            }

            IsLogin = true;
            return (memberId, passHash);
        }

        private const string MemberIdCookie = "ipb_member_id";
        private const string PassHashCookie = "ipb_pass_hash";

        public void RestoreLogin(string memberId, string passHash)
        {
            if (IsLogin)
                throw new InvalidOperationException("Logout first.");

            _cookies.Add(new Cookie(MemberIdCookie, memberId, null, "e-hentai.org"));
            _cookies.Add(new Cookie(PassHashCookie, passHash, null, "e-hentai.org"));

            _cookies.Add(new Cookie(MemberIdCookie, memberId, null, "forums.e-hentai.org"));
            _cookies.Add(new Cookie(PassHashCookie, passHash, null, "forums.e-hentai.org"));

            _cookies.Add(new Cookie(MemberIdCookie, memberId, null, "exhentai.org"));
            _cookies.Add(new Cookie(PassHashCookie, passHash, null, "exhentai.org"));

            IsLogin = true;
        }

        public void Logout()
        {
            if (!IsLogin)
                throw new InvalidOperationException("Login first.");

            RemoveCookies("https://e-hentai.org");
            RemoveCookies("https://forums.e-hentai.org");
            RemoveCookies("https://exhentai.org");
            IsLogin = false;

            void RemoveCookies(string domain)
            {
                var cookies = _cookies.GetCookies(new Uri(domain));
                var memberId = cookies[MemberIdCookie];
                if (memberId is not null)
                    cookies.Remove(memberId);
                var passHash = cookies[PassHashCookie];
                if (passHash is not null)
                    cookies.Remove(passHash);
            }
        }

        private static readonly Regex s_galleryRegex
            = new(@"e[x\-]hentai.org/g/(\d+)/([0-9a-f]+)", RegexOptions.Compiled | RegexOptions.ECMAScript | RegexOptions.CultureInvariant);
        private static readonly JsonSerializerOptions s_apiResponseOptions = new(JsonSerializerDefaults.Web);

        private async Task<ImmutableArray<GalleryMetadata>> RequestApiAsync(
            IEnumerable<(int GId, string Token)> galleryIds,
            CancellationToken cancellationToken = default)
        {
            var content = JsonContent.Create(new
            {
                method = "gdata",
                gidlist = galleryIds.Select(x => (new object[] { x.GId, x.Token })),
                @namespace = 1
            });
            await content.LoadIntoBufferAsync().ConfigureAwait(false);
            using var apiResponse = await HttpClient.PostAsync("https://api.e-hentai.org/api.php", content, cancellationToken).ConfigureAwait(false);
            var rsp = await apiResponse.Content.ReadFromJsonAsync<EHentaiApiResponse>(s_apiResponseOptions, cancellationToken).ConfigureAwait(false);

            if (rsp is null)
                throw new InvalidOperationException("Empty api response.");
            if (rsp.Error != null)
                throw new InvalidOperationException(rsp.Error);

            return rsp.Galleries;
        }

        private static (int Gid, string Token) GetGalleryIdFromUri(string uri)
        {
            var match = s_galleryRegex.Match(uri);
            int gid = int.Parse(match.Groups[1].Value, NumberFormatInfo.InvariantInfo);
            string token = match.Groups[2].Value;
            return (gid, token);
        }

        public async Task<GalleryListPage> GetPageAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            using var document = await OpenDocumentAsync(uri, cancellationToken).ConfigureAwait(false);
            var table = document.QuerySelector<IHtmlTableElement>("table.itg");

            var galleries = document
                .QuerySelector<IHtmlTableElement>("table.itg")!
                .QuerySelectorAll<IHtmlAnchorElement>("td.glname>a")
                .Select(a => GetGalleryIdFromUri(a.Href))
                .ToArray();

            int totalCount = 0;
            int pagesCount = 0;
            if (galleries.Length > 0)
            {
                string countText = document.QuerySelector("div.ido>div>p.ip")!.Text(); // Showing xxx results
                totalCount = int.Parse(countText.AsSpan()[8..^8], NumberStyles.AllowThousands);

                pagesCount = int.Parse(
                    document
                        .QuerySelector("table.ptt")!
                        .QuerySelectorAll("td")[^2]
                        .QuerySelector("a")!
                        .Text(),
                    null);
            }

            var apiGalleries = await RequestApiAsync(galleries, cancellationToken).ConfigureAwait(false);

            return new(totalCount, pagesCount,
                apiGalleries.Select(g => new Gallery(this, uri, g)).ToImmutableArray());
        }

        public Task<GalleryListPage> GetPageAsync(ListRequest _, int page = 0, CancellationToken cancellationToken = default)
        {
            string host = UseExHentai ? "exhentai.org" : "e-hentai.org";
            string uri = $"https://{host}/?page={page}";
            return GetPageAsync(new Uri(uri), cancellationToken);
        }

        public async Task<Gallery> GetGalleryAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            var (gid, token) = GetGalleryIdFromUri(uri.ToString());
            var galleries = await RequestApiAsync(new[] { (gid, token) }, cancellationToken).ConfigureAwait(false);
            return new(this, uri, galleries.Single());
        }
    }
}
