using System;
using System.Net;
using System.Net.Http;

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
    }
}
