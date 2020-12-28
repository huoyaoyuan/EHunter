using System;
using System.Threading.Tasks;
using EHunter.Provider.Pixiv.Messages;
using EHunter.Provider.Pixiv.Models;
using EHunter.Provider.Pixiv.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
    public class PixivLoginPageVM : ObservableObject
    {
        private readonly PixivSettingStore _settingStore;
        private readonly PixivSetting2 _setting;
        private readonly PixivClientService _clientService;

        public PixivLoginPageVM(
            PixivSettingStore settingStore,
            PixivSetting2 setting,
            PixivClientService clientService,
            DatabaseAccessor databaseAccessor)
        {
            _settingStore = settingStore;
            _setting = setting;
            _clientService = clientService;

            _useProxy = setting.UseProxy;

            string? savedToken = settingStore.RefreshToken;
            if (!string.IsNullOrEmpty(savedToken))
            {
                RefreshToken = savedToken;
                LoginWithToken();
            }

            InitDatabase();
            async void InitDatabase()
            {
                var factory = await databaseAccessor.FactoryTask.ConfigureAwait(false);
                DatabaseInitState = factory is not null;
                CheckInitialize();
            }
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

        private bool _isLoggingIn;
        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            private set => SetProperty(ref _isLoggingIn, value);
        }

        private bool _isLoggedin;
        public bool IsLoggedin
        {
            get => _isLoggedin;
            private set => SetProperty(ref _isLoggedin, value);
        }

        private bool _useProxy;
        public bool UseProxy
        {
            get => _useProxy;
            set
            {
                if (SetProperty(ref _useProxy, value))
                    _setting.SetUseProxy(value);
            }
        }

        public void LoginWithPassword() => PerformLogin(_clientService.LoginAsync(Username, Password));

        public void LoginWithToken() => PerformLogin(_clientService.LoginAsync(RefreshToken));

        private async void PerformLogin(Task<string> loginTask)
        {
            IsLoggingIn = true;

            try
            {
                string refreshToken = await loginTask.ConfigureAwait(true);
                _settingStore.RefreshToken = refreshToken;
                IsLoggedin = true;
            }
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new LoginFailedMessage(ex));
            }

            IsLoggingIn = false;
            CheckInitialize();
        }

        private bool? _databaseInitState;
        public bool? DatabaseInitState
        {
            get => _databaseInitState;
            private set => SetProperty(ref _databaseInitState, value);
        }

        private void CheckInitialize()
        {
            if (IsLoggedin && DatabaseInitState != null)
            {
                WeakReferenceMessenger.Default.Send(new InitializationCompleteMessage());
            }
        }
    }
}
