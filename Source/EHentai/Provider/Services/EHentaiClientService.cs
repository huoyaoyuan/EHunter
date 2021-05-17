using System;
using System.Composition;
using System.Net.Http;
using System.Reactive.Linq;
using EHunter.DependencyInjection;
using EHunter.EHentai.Api;
using EHunter.EHentai.Settings;
using EHunter.Settings;

namespace EHunter.EHentai.Services
{
    [Export, Export(typeof(ICustomResolver<EHentaiClient>)), Shared]
    public sealed class EHentaiClientService : ICustomResolver<EHentaiClient>, IDisposable
    {
        private readonly IDisposable _subscribeDisposable;
        private readonly EHentaiClient _client = new();

        [ImportingConstructor]
        public EHentaiClientService(IProxySetting proxySetting, EHentaiSetting eHentaiSetting, IEHentaiSettingStore settingStore)
        {
            _subscribeDisposable = proxySetting.Proxy
                .CombineLatest(eHentaiSetting.ConnectionMode,
                    (proxy, mode) => mode switch
                    {
                        EHentaiConnectionMode.ApplicationProxy => proxy,
                        EHentaiConnectionMode.SystemProxy => HttpClient.DefaultProxy,
                        EHentaiConnectionMode.NoProxy => null,
                        _ => throw new InvalidOperationException($"Unknown enum value {mode}.")
                    })
                .Subscribe(proxy => _client.SetProxy(proxy));

            string? memberId = settingStore.MemberId;
            string? passHash = settingStore.PassHash;
            if (memberId is not null && passHash is not null)
                _client.RestoreLogin(memberId, passHash);
        }

        public void Dispose()
        {
            _subscribeDisposable.Dispose();
            _client.Dispose();
        }

        public EHentaiClient Resolve() => _client;
    }
}
