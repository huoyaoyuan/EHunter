using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.EHentai.Api;
using EHunter.EHentai.Settings;

namespace EHunter.EHentai.ViewModels
{
    [Export, Shared]
    public class EHentaiSettingsVM : ObservableObject
    {
        private readonly EHentaiSetting _settings;
        private readonly IEHentaiSettingStore _settingStore;
        private readonly ICustomResolver<EHentaiClient> _clientResolver;

        [ImportingConstructor]
        public EHentaiSettingsVM(EHentaiSetting settings, IEHentaiSettingStore settingStore, ICustomResolver<EHentaiClient> clientResolver)
        {
            _settings = settings;
            _settingStore = settingStore;
            _clientResolver = clientResolver;

            _connectionMode = settingStore.ConnectionMode;
            _useExHentai = settingStore.UseExHentai;
            IsLogin = clientResolver.Resolve().IsLogin;
        }

        private bool _isLogin;
        public bool IsLogin
        {
            get => _isLogin;
            private set => SetProperty(ref _isLogin, value);
        }

        private bool _isLoggingIn;
        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            private set => SetProperty(ref _isLoggingIn, value);
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
                IsLoggingIn = true;
                var client = _clientResolver.Resolve();
                var (memberId, passHash) = await client.LoginAsync(Username, Password).ConfigureAwait(true);
                _settingStore.MemberId = memberId;
                _settingStore.PassHash = passHash;
                IsLogin = client.IsLogin;
            }
            catch
            {

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
