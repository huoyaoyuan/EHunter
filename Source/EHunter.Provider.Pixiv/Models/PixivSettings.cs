using System;
using System.Threading.Tasks;
using EHunter.Provider.Pixiv.Messages;
using EHunter.Settings;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Windows.Storage;

namespace EHunter.Provider.Pixiv.Models
{
#pragma warning disable CA1067 // 在实现 IEquatable<T> 时替代 Object.Equals(object)
    public sealed class PixivSettings : ObservableObject, IDisposable, IEquatable<PixivSettings>
#pragma warning restore CA1067 // 在实现 IEquatable<T> 时替代 Object.Equals(object)
    {
        private readonly ApplicationDataContainer _applicationSetting;
        private readonly ICommonSetting _commonSetting;
        private readonly PixivCacheService _cache;

        public PixivSettings(ICommonSetting commonSetting, PixivCacheService cache)
        {
            _applicationSetting = ApplicationData.Current.LocalSettings.CreateContainer("Pixiv", ApplicationDataCreateDisposition.Always);

            _useProxy = (bool?)_applicationSetting.Values[nameof(UseProxy)] ?? false;
            _commonSetting = commonSetting;
            _cache = cache;
            Client = new PixivClient(_useProxy ? _commonSetting.Proxy : null);
            _commonSetting.ProxyUpdated += p => Client.SetProxy(_useProxy ? p : null);

            if (_applicationSetting.Values["RefreshToken"] is string refreshToken)
            {
                RefreshToken = refreshToken;
                LoginWithToken();
            }
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
                    Client.SetProxy(_useProxy ? _commonSetting.Proxy : null);
                }
            }
        }

        internal PixivClient Client { get; }

        public void Dispose() => Client.Dispose();

        private bool _isLoggingIn;
        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            private set => SetProperty(ref _isLoggingIn, value);
        }

        private bool _isLogin;
        public bool IsLogin
        {
            get => _isLogin;
            private set => SetProperty(ref _isLogin, value);
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

        private LoginUser? _user;
        public LoginUser? User
        {
            get => _user;
            private set => SetProperty(ref _user, value);
        }

        private Uri? _userAvatarUri;
        public Uri? UserAvatarUri
        {
            get => _userAvatarUri;
            set => SetProperty(ref _userAvatarUri, value);
        }

        public void LoginWithPassword() => PerformLogin(Client.LoginAsync(Username, Password));

        public void LoginWithToken() => PerformLogin(Client.LoginAsync(RefreshToken));

        private async void PerformLogin(Task<string> loginTask)
        {
            IsLoggingIn = true;

            try
            {
                string refreshToken = await loginTask.ConfigureAwait(true);

                if (Client.IsLogin)
                {
                    User = Client.CurrentUser;
                    IsLogin = true;
                    _applicationSetting.Values["RefreshToken"] = refreshToken;
                }
            }
            catch (Exception e)
            {
                Messenger.Default.Send(new LoginFailedMessage(e), this);
            }

            IsLoggingIn = false;

            if (Client.IsLogin)
            {
                using var response = await Client.CurrentUser.GetAvatarAsync().ConfigureAwait(true);
                UserAvatarUri = await _cache.GetLocalFileAsync(response).ConfigureAwait(true);
            }
        }

        public bool Equals(PixivSettings? other) => ReferenceEquals(this, other);
    }
}
