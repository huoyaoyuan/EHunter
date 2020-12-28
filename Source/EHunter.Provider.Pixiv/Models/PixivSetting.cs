using System;
using EHunter.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.Models
{
    public class PixivSetting
    {
        private readonly PixivSettingStore _settingStore;

        public PixivSetting(PixivSettingStore settingStore)
        {
            _settingStore = settingStore;
            _useProxy = new ObservableProperty<bool>(_settingStore.UseProxy);
        }

        private readonly ObservableProperty<bool> _useProxy;
        public bool UseProxy => _useProxy.Value;
        public IObservable<bool> UseProxyChanged => _useProxy.ValueObservable;

        public void SetUseProxy(bool value) => _useProxy.Value = value;
    }
}
