﻿using System.Reactive.Linq;
using EHunter.DependencyInjection;
using EHunter.EHentai.Api;
using EHunter.EHentai.Settings;
using EHunter.Settings;

namespace EHunter.EHentai.Services
{
    public sealed class EHentaiClientService : ICustomResolver<EHentaiClient>, IDisposable
    {
        private readonly IDisposable _connectionDisposable, _endpointDisposable;
        private readonly EHentaiClient _client = new();

        public EHentaiClientService(IProxySetting proxySetting, EHentaiSetting eHentaiSetting, IEHentaiSettingStore settingStore)
        {
            _connectionDisposable = proxySetting.Proxy
                .CombineLatest(eHentaiSetting.ConnectionMode,
                    (proxy, mode) => mode switch
                    {
                        EHentaiConnectionMode.ApplicationProxy => proxy,
                        EHentaiConnectionMode.SystemProxy => HttpClient.DefaultProxy,
                        EHentaiConnectionMode.NoProxy => null,
                        _ => throw new InvalidOperationException($"Unknown enum value {mode}.")
                    })
                .Subscribe(proxy => _client.SetProxy(proxy));

            _endpointDisposable = eHentaiSetting.UseExHentai
                .Subscribe(value => _client.UseExHentai = value);

            string? memberId = settingStore.MemberId;
            string? passHash = settingStore.PassHash;
            if (memberId is not null && passHash is not null)
                _client.RestoreLogin(memberId, passHash);
        }

        public void Dispose()
        {
            _connectionDisposable.Dispose();
            _endpointDisposable.Dispose();
            _client.Dispose();
        }

        public EHentaiClient Resolve() => _client;
    }
}
