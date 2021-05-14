using System;
using System.Composition;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EHunter.Data;
using EHunter.Pixiv.Data;
using EHunter.Pixiv.Messages;
using EHunter.Pixiv.Services;
using EHunter.Pixiv.Settings;

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

        public void LoginWithWebView(Func<string, Task<Uri>> browserTask) => PerformLogin(_clientService.LoginAsync(browserTask));

        public void LoginWithToken() => PerformLogin(_clientService.LoginAsync(RefreshToken));

        private bool _showLoginException;
        public bool ShowLoginException
        {
            get => _showLoginException;
            set => SetProperty(ref _showLoginException, value);
        }

        private Exception? _loginException;
        public Exception? LoginException
        {
            get => _loginException;
            set => SetProperty(ref _loginException, value);
        }

        private async void PerformLogin(Task<string> loginTask)
        {
            IsLoggingIn = true;
            ShowLoginException = false;
            LoginException = null;

            try
            {
                string refreshToken = await loginTask.ConfigureAwait(true);
                _settingStore.RefreshToken = refreshToken;
                IsLoggedin = true;
            }
            catch (Exception ex)
            {
                LoginException = ex;
                ShowLoginException = true;
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
