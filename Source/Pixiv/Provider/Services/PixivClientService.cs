using System.Composition;
using System.Reactive.Linq;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Settings;
using EHunter.Settings;
using Meowtrix.PixivApi;

namespace EHunter.Pixiv.Services
{
    [Export(typeof(ICustomResolver<PixivClient>)), Shared]
    public sealed class PixivClientService : ICustomResolver<PixivClient>, IDisposable
    {
        private readonly PixivClient _client = new();
        private readonly IDisposable _subscribeDisposable;

        [ImportingConstructor]
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
                .Subscribe(h => _client.SetHandler(h));
        }

        public PixivClient Resolve() => _client;

        public void Dispose()
        {
            _subscribeDisposable.Dispose();
            _client.Dispose();
        }
    }
}
