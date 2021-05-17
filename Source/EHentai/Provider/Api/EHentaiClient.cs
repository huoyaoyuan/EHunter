﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EHunter.EHentai.Api.Json;
using EHunter.EHentai.Api.Models;

namespace EHunter.EHentai.Api
{
    public sealed class EHentaiClient : IDisposable
    {
        private HttpClient _httpClient;
        private readonly CookieContainer _cookies = new();

        public EHentaiClient(IWebProxy? proxy = null)
            => _httpClient = CreateNewClient(proxy);

        public void SetProxy(IWebProxy? proxy)
        {
            _httpClient.Dispose();
            _httpClient = CreateNewClient(proxy);
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

        public void Dispose() => _httpClient.Dispose();

        public async Task LoginAsync(string username, string password)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://forums.e-hentai.org/index.php?act=Login&CODE=01")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "UserName", username },
                    { "PassWord", password },
                    { "CookieDate", "1" },
                }!)
            };

            await _httpClient.SendAsync(request).ConfigureAwait(false);
        }

        private const string MemberIdCookie = "ipb_member_id";
        private const string PassHashCookie = "ipb_pass_hash";

        public (string memberId, string passHash) SaveLogin()
        {
            var cookies = _cookies.GetCookies(new Uri("https://e-hentai.org"));
            return (cookies[MemberIdCookie]?.Value ?? throw new InvalidOperationException("No login cookies"),
                cookies[PassHashCookie]?.Value ?? throw new InvalidOperationException("No login cookies"));
        }

        public void RestoreLogin(string memberId, string passHash)
        {
            _cookies.Add(new Cookie(MemberIdCookie, memberId, null, "e-hentai.org"));
            _cookies.Add(new Cookie(PassHashCookie, passHash, null, "e-hentai.org"));

            _cookies.Add(new Cookie(MemberIdCookie, memberId, null, "forums.e-hentai.org"));
            _cookies.Add(new Cookie(PassHashCookie, passHash, null, "forums.e-hentai.org"));

            _cookies.Add(new Cookie(MemberIdCookie, memberId, null, "exhentai.org"));
            _cookies.Add(new Cookie(PassHashCookie, passHash, null, "exhentai.org"));
        }

        public void Logout()
        {
            RemoveCookies("https://e-hentai.org");
            RemoveCookies("https://forums.e-hentai.org");
            RemoveCookies("https://exhentai.org");

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

        public async Task<IReadOnlyList<Gallery>> GetPageAsync(Uri uri)
        {
            using var request = await _httpClient.GetStreamAsync(uri).ConfigureAwait(false);

            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(request)).ConfigureAwait(false);
            var table = document.QuerySelector<IHtmlTableElement>("table.itg");

            var galleries = table.Rows.Skip(1).Select(r =>
            {
                string url = r.QuerySelector("td.glname")
                    .QuerySelector<IHtmlAnchorElement>("a")
                    .Href;
                var match = s_galleryRegex.Match(url);
                int gid = int.Parse(match.Groups[1].Value, NumberFormatInfo.InvariantInfo);
                string token = match.Groups[2].Value;
                return new object[] { gid, token };
            }).ToArray();

            var apiRequest = new
            {
                method = "gdata",
                gidlist = galleries,
                @namespace = 1
            };
            var content = JsonContent.Create(apiRequest);
            await content.LoadIntoBufferAsync().ConfigureAwait(false);
            using var apiResponse = await _httpClient.PostAsync("https://api.e-hentai.org/api.php", content).ConfigureAwait(false);
            var rsp = await apiResponse.Content.ReadFromJsonAsync<EHentaiApiResponse>(s_apiResponseOptions).ConfigureAwait(false);

            if (rsp is null)
                throw new InvalidOperationException("Empty api response.");
            if (rsp.Error != null)
                throw new InvalidOperationException(rsp.Error);

            return rsp.Galleries.Select(g => new Gallery(this, uri, g)).ToArray();
        }
    }
}
