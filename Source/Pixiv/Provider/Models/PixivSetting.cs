﻿using System;
using EHunter.ComponentModel;

namespace EHunter.Pixiv.Models
{
    public class PixivSetting
    {
        private readonly IPixivSettingStore _settingStore;

        public PixivSetting(IPixivSettingStore settingStore)
        {
            _settingStore = settingStore;
            _useProxy = new ObservableProperty<bool>(_settingStore.UseProxy);
            _maxDownloadsInParallel = new ObservableProperty<int>(_settingStore.MaxDownloadsInParallel);
        }

        private readonly ObservableProperty<bool> _useProxy;
        public bool UseProxy => _useProxy.Value;
        public IObservable<bool> UseProxyChanged => _useProxy.ValueObservable;

        public void SetUseProxy(bool value)
        {
            _useProxy.Value = value;
            _settingStore.UseProxy = value;
        }

        private readonly ObservableProperty<int> _maxDownloadsInParallel;
        public int MaxDownloadsInParallel => _maxDownloadsInParallel.Value;
        public IObservable<int> MaxDownloadsInParallelChanged => _maxDownloadsInParallel.ValueObservable;

        public void SetMaxDownloadsInParallel(int value)
        {
            _settingStore.MaxDownloadsInParallel = value;
            _maxDownloadsInParallel.Value = value;
        }
    }
}