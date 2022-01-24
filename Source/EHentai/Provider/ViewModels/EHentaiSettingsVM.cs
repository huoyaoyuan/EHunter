using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.EHentai.Api;
using EHunter.EHentai.Settings;

namespace EHunter.EHentai.ViewModels
{
    public partial class EHentaiSettingsVM : ObservableObject
    {
        private readonly EHentaiSetting _settings;
        private readonly IEHentaiSettingStore _settingStore;
        private readonly ICustomResolver<EHentaiClient> _clientResolver;

        public EHentaiSettingsVM(EHentaiSetting settings, IEHentaiSettingStore settingStore, ICustomResolver<EHentaiClient> clientResolver)
        {
            _settings = settings;
            _settingStore = settingStore;
            _clientResolver = clientResolver;

            _connectionMode = settingStore.ConnectionMode;
            _useExHentai = settingStore.UseExHentai;
            IsLogin = clientResolver.Resolve().IsLogin;
        }

        [ObservableProperty]
        private bool _isLogin;

        [ObservableProperty]
        private bool _isLoggingIn;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _showLoginException;

        [ObservableProperty]
        private Exception? _loginException;

        private EHentaiConnectionMode _connectionMode;
        public EHentaiConnectionMode ConnectionMode
        {
            get => _connectionMode;
            set
            {
                if (SetProperty(ref _connectionMode, value))
                    _settings.SetConnectionOption(value);
            }
        }

        private bool _useExHentai;
        public bool UseExHentai
        {
            get => _useExHentai;
            set
            {
                if (SetProperty(ref _useExHentai, value))
                    _settings.SetUseExHentai(value);
            }
        }

        public async void Login()
        {
            try
            {
                ShowLoginException = false;
                LoginException = null;
                IsLoggingIn = true;
                var client = _clientResolver.Resolve();
                var (memberId, passHash) = await client.LoginAsync(Username, Password).ConfigureAwait(true);
                _settingStore.MemberId = memberId;
                _settingStore.PassHash = passHash;
                IsLogin = client.IsLogin;
            }
            catch (Exception ex)
            {
                LoginException = ex;
                ShowLoginException = true;
            }
            finally
            {
                IsLoggingIn = false;
            }
        }

        public void Logout()
        {
            var client = _clientResolver.Resolve();
            client.Logout();
            IsLogin = false;
            _settingStore.MemberId = null;
            _settingStore.PassHash = null;
        }
    }
}
