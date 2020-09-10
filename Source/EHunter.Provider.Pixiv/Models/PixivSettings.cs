using System;
using System.Threading.Tasks;
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

        private bool _isLoggingIn;
        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set => SetProperty(ref _isLoggingIn, value);
        }

        private bool _isLogin;
        public bool IsLogin
        {
            get => _isLogin;
            set => SetProperty(ref _isLogin, value);
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _refreshToken = string.Empty;
        public string RefreshToken
        {
            get => _refreshToken;
            set => SetProperty(ref _refreshToken, value);
        }

        public async void LoginWithPassword()
        {
            IsLoggingIn = true;
            await Task.Delay(1000).ConfigureAwait(true);
            IsLoggingIn = false;
        }

        public async void LoginWithToken()
        {
            IsLoggingIn = true;
            await Task.Delay(1000).ConfigureAwait(true);
            IsLoggingIn = false;
        }
    }
}
