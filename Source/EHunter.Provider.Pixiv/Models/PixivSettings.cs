using System;
using System.Collections.Generic;
using EHunter.Settings;
using Meowtrix.PixivApi;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace EHunter.Provider.Pixiv.Models
{
    public sealed class PixivSettings : ObservableObject, IDisposable
    {
        private readonly ApplicationDataContainer _applicationSetting;
        private readonly ICommonSetting _commonSetting;

        public PixivSettings(ICommonSetting commonSetting)
        {
            _applicationSetting = ApplicationData.Current.LocalSettings;

            _useProxy = (bool?)_applicationSetting.Values[nameof(UseProxy)] ?? false;
            _commonSetting = commonSetting;

            Client = new PixivClient(_useProxy);
        }

        private bool _useProxy;
        public bool UseProxy
        {
            get => _useProxy;
            set
            {
                if (SetProperty(ref _useProxy, value))
                {
                    _applicationSetting.Values[nameof(UseProxy)] = value;
                    if (value)
                        Client.SetDefaultProxy();
                    else
                        Client.SetProxy(null);
                }
            }
        }

        internal PixivClient Client { get; }

        public void Dispose() => Client.Dispose();

        private bool _isLogin;
        public bool IsLogin
        {
            get => _isLogin;
            set => SetProperty(ref _isLogin, value);
        }
    }
}
