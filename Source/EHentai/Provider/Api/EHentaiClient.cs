using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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

        public (string memberId, string passHash) SaveLogin()
        {
            var cookies = _cookies.GetCookies(new Uri("https://e-hentai.org"));
            return (cookies["ipb_member_id"]?.Value ?? throw new InvalidOperationException("No login cookies"),
                cookies["ipb_pass_hash"]?.Value ?? throw new InvalidOperationException("No login cookies"));
        }

        public void RestoreLogin(string memberId, string passHash)
        {
            _cookies.Add(new Cookie("ipb_member_id", memberId, null, "e-hentai.org"));
            _cookies.Add(new Cookie("ipb_pass_hash", passHash, null, "e-hentai.org"));

            _cookies.Add(new Cookie("ipb_member_id", memberId, null, "forums.e-hentai.org"));
            _cookies.Add(new Cookie("ipb_pass_hash", passHash, null, "forums.e-hentai.org"));

            _cookies.Add(new Cookie("ipb_member_id", memberId, null, "exhentai.org"));
            _cookies.Add(new Cookie("ipb_pass_hash", passHash, null, "exhentai.org"));
        }
    }
}
