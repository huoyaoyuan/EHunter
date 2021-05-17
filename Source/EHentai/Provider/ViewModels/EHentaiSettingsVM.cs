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
            string? memberId = settingStore.MemberId;
            string? passHash = settingStore.PassHash;
            if (memberId is not null && passHash is not null)
            {
                _clientResolver.Resolve().RestoreLogin(memberId, passHash);
                IsLogin = true;
            }
        }

        private bool _isLogin;
        public bool IsLogin
        {
            get => _isLogin;
            set => SetProperty(ref _isLogin, value);
        }

        private bool _isLoggingIn;
        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set => SetProperty(ref _isLoggingIn, value);
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

        public int IntConnectionMode
        {
            get => (int)ConnectionMode;
            set => ConnectionMode = (EHentaiConnectionMode)value;
        }

        public async void Login()
        {
            try
            {
                IsLoggingIn = true;
                var client = _clientResolver.Resolve();
                await client.LoginAsync(Username, Password).ConfigureAwait(true);
                var (memberId, passHash) = client.SaveLogin();
                _settingStore.MemberId = memberId;
                _settingStore.PassHash = passHash;
            }
            catch
            {

            }
            finally
            {
                IsLoggingIn = false;
            }
        }
    }
}
