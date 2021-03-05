using System;
using System.Composition;
using System.Threading.Tasks;
using EHunter.Data;
using EHunter.Pixiv.Data;
using EHunter.Pixiv.Messages;
using EHunter.Pixiv.Services;
using EHunter.Pixiv.Settings;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace EHunter.Pixiv.ViewModels
{
    [Export]
    public class LoginPageVM : ObservableObject
    {
        private readonly IPixivSettingStore _settingStore;
        private readonly PixivSetting _setting;
        private readonly PixivClientService _clientService;

        [ImportingConstructor]
        public LoginPageVM(
            IPixivSettingStore settingStore,
            PixivSetting setting,
            PixivClientService clientService,
            EHunterDbContextResolver<EHunterDbContext> eHunterContextResolver,
            EHunterDbContextResolver<PixivDbContext> pixivContextResolver)
        {
            _settingStore = settingStore;
            _setting = setting;
            _clientService = clientService;

            _connectionMode = _setting.ConnectionMode.Value;

            string? savedToken = settingStore.RefreshToken;
            if (!string.IsNullOrEmpty(savedToken))
            {
                RefreshToken = savedToken;
                LoginWithToken();
            }

            InitDatabase();
            async void InitDatabase()
            {
                await Task.WhenAll(eHunterContextResolver.InitializeTask, pixivContextResolver.InitializeTask).ConfigureAwait(true);
                DatabaseInitState = eHunterContextResolver.Resolve() is not null && pixivContextResolver.Resolve() is not null;
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

        private PixivConnectionMode _connectionMode;
        public PixivConnectionMode ConnectionMode
        {
            get => _connectionMode;
            set
            {
                if (SetProperty(ref _connectionMode, value))
                {
                    OnPropertyChanged(nameof(IntConnectionMode));
                    _setting.SetConnectionOption(value);
                }
            }
        }

        public int IntConnectionMode
        {
            get => (int)ConnectionMode;
            set => ConnectionMode = (PixivConnectionMode)value;
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
