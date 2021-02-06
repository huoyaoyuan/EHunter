using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Settings;
using EHunter.Settings;
using Meowtrix.PixivApi;

namespace EHunter.Pixiv.Services
{
    public sealed class PixivClientService : ICustomResolver<PixivClient>, IDisposable
    {
        private PixivClient? _client;
        private HttpMessageHandler _effectivehandler;
        private readonly IDisposable _subscribeDisposable;

        public PixivClientService(IProxySetting proxySetting, PixivSetting pixivSetting)
        {
            _subscribeDisposable = proxySetting.Proxy
                .CombineLatest(pixivSetting.ConnectionMode,
                    (proxy, mode) => mode switch
                    {
                        PixivConnectionMode.SystemProxy
                            => new HttpClientHandler
                            {
                                UseProxy = true,
                                Proxy = HttpClient.DefaultProxy
                            },
                        PixivConnectionMode.ApplicationProxy
                            => new HttpClientHandler
                            {
                                UseProxy = true,
                                Proxy = proxy
                            },
                        PixivConnectionMode.NoProxy
                            => new HttpClientHandler
                            {
                                UseProxy = false
                            },
                        PixivConnectionMode.DirectConnect
                            => (HttpMessageHandler)new DirectConnectHandler(),
                        _ => throw new InvalidOperationException($"Unknown enum value {mode}.")
                    })
                .Subscribe(h =>
                {
                    _effectivehandler = h;
                    _client?.SetHandler(h);
                });
            Debug.Assert(_effectivehandler != null);
        }

        public Task<string> LoginAsync(string username, string password)
            => LoginAsync(c => c.LoginAsync(username, password));

        public Task<string> LoginAsync(string refreshToken)
            => LoginAsync(c => c.LoginAsync(refreshToken));

        private async Task<string> LoginAsync(Func<PixivClient, Task<string>> loginMethod)
        {
            var client = new PixivClient(_effectivehandler);
            string refreshToken = await loginMethod(client).ConfigureAwait(false);

            var oldClient = _client;
            _client = client;
            oldClient?.Dispose();

            return refreshToken;
        }

        public PixivClient Resolve()
            => _client ?? throw new InvalidOperationException("This service should be called after initialization.");

        public void Dispose()
        {
            _subscribeDisposable.Dispose();
            _client?.Dispose();
        }
    }
}
