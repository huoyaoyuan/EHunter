using System;
using System.Net.Http;
using System.Threading.Tasks;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Models;
using EHunter.Settings;
using Meowtrix.PixivApi;

namespace EHunter.Pixiv.Services
{
    public sealed class PixivClientService : ICustomResolver<PixivClient>, IDisposable
    {
        private PixivClient? _client;

        private readonly IProxySetting _proxySetting;
        private readonly IDisposable _proxySettingDisposable;

        private readonly PixivSetting _pixivSetting;
        private readonly IDisposable _pixivSettingDisposable;

        public PixivClientService(IProxySetting proxySetting, PixivSetting pixivSetting)
        {
            _proxySetting = proxySetting;
            _pixivSetting = pixivSetting;

            _proxySettingDisposable = proxySetting.Proxy.Subscribe(_ => UpdateEffectiveProxy(_client));
            _pixivSettingDisposable = pixivSetting.UseProxy.Subscribe(_ => UpdateEffectiveProxy(_client));
        }

        private void UpdateEffectiveProxy(PixivClient? client)
        {
            if (client is null)
                return;
            var handler = GetEffectiveHandler();

            client.SetHandler(handler);
        }

        private HttpClientHandler GetEffectiveHandler()
        {
            return new HttpClientHandler
            {
                UseProxy = _pixivSetting.UseProxy.Value,
                Proxy = _proxySetting.Proxy.Value
            };
        }

        public Task<string> LoginAsync(string username, string password)
            => LoginAsync(c => c.LoginAsync(username, password));

        public Task<string> LoginAsync(string refreshToken)
            => LoginAsync(c => c.LoginAsync(refreshToken));

        private async Task<string> LoginAsync(Func<PixivClient, Task<string>> loginMethod)
        {
            var client = new PixivClient(GetEffectiveHandler());
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
            _proxySettingDisposable.Dispose();
            _pixivSettingDisposable.Dispose();
            _client?.Dispose();
        }
    }
}
