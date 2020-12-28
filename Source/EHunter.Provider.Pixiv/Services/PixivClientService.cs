using System;
using System.Threading.Tasks;
using EHunter.DependencyInjection;
using EHunter.Provider.Pixiv.Models;
using EHunter.Settings;
using Meowtrix.PixivApi;

#nullable enable

namespace EHunter.Provider.Pixiv.Services
{
    public sealed class PixivClientService : ICustomResolver<PixivClient>, IDisposable
    {
        private PixivClient? _client;

        private readonly IProxySetting _proxySetting;
        private IDisposable? _proxySettingDisposable;

        private readonly PixivSetting2 _pixivSetting;
        private IDisposable? _pixivSettingDisposable;

        private readonly object _propertyLock = new();

        public PixivClientService(IProxySetting proxySetting, PixivSetting2 pixivSetting)
        {
            _proxySetting = proxySetting;
            _pixivSetting = pixivSetting;
        }

        public Task<string> LoginAsync(string username, string password)
            => LoginAsync(c => c.LoginAsync(username, password));

        public Task<string> LoginAsync(string refreshToken)
            => LoginAsync(c => c.LoginAsync(refreshToken));

        private async Task<string> LoginAsync(Func<PixivClient, Task<string>> loginMethod)
        {
            var client = new PixivClient(_pixivSetting.UseProxy ? _proxySetting.WebProxy : null);
            string refreshToken = await loginMethod(client).ConfigureAwait(false);

            lock (_propertyLock)
            {
                var oldClient = _client;
                _client = client;

                oldClient?.Dispose();

                _proxySettingDisposable?.Dispose();
                _proxySettingDisposable = _proxySetting.ProxyChanged.Subscribe(p =>
                    client.SetProxy(_pixivSetting.UseProxy ? p : null));

                _pixivSettingDisposable?.Dispose();
                _pixivSettingDisposable = _pixivSetting.UseProxyChanged.Subscribe(use =>
                    client.SetProxy(use ? _proxySetting.WebProxy : null));
            }

            return refreshToken;
        }

        public PixivClient Resolve()
            => _client ?? throw new InvalidOperationException("This service should be called after initialization.");

        public void Dispose()
        {
            _proxySettingDisposable?.Dispose();
            _pixivSettingDisposable?.Dispose();
            _client?.Dispose();
        }
    }
}
