using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EHunter.Data;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Data;
using EHunter.Pixiv.Messages;
using EHunter.Pixiv.Settings;
using Meowtrix.PixivApi;

namespace EHunter.Pixiv.ViewModels
{
    [Export]
    public partial class LoginPageVM : ObservableObject
    {
        private readonly IPixivSettingStore _settingStore;
        private readonly PixivSetting _setting;
        private readonly PixivClient _client;

        [ObservableProperty]
        private string _refreshToken = string.Empty;

        // TODO: Custom source generation
        private PixivConnectionMode _connectionMode;
        public PixivConnectionMode ConnectionMode
        {
            get => _connectionMode;
            set
            {
                if (SetProperty(ref _connectionMode, value))
                    _setting.SetConnectionOption(value);
            }
        }

        [ObservableProperty]
        private bool _isLoggingIn;

        [ObservableProperty]
        private bool _isLoggedin;

        [ObservableProperty]
        private bool? _databaseInitState;

        [ObservableProperty]
        private bool _showLoginException;

        [ObservableProperty]
        private Exception? _loginException;

        [ImportingConstructor]
        public LoginPageVM(
            IPixivSettingStore settingStore,
            PixivSetting setting,
            ICustomResolver<PixivClient> clientService,
            EHunterDbContextResolver<EHunterDbContext> eHunterContextResolver,
            EHunterDbContextResolver<PixivDbContext> pixivContextResolver)
        {
            _settingStore = settingStore;
            _setting = setting;
            _client = clientService.Resolve();

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

        public void LoginWithWebView(Func<string, Task<Uri>> browserTask) => PerformLogin(_client.LoginAsync(browserTask));

        public void LoginWithToken() => PerformLogin(_client.LoginAsync(RefreshToken));

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

        private void CheckInitialize()
        {
            if (IsLoggedin && DatabaseInitState != null)
            {
                WeakReferenceMessenger.Default.Send(new InitializationCompleteMessage());
            }
        }
    }
}
