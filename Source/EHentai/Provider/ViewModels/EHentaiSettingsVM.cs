using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.EHentai.Api;
using EHunter.EHentai.Settings;

namespace EHunter.EHentai.ViewModels
{
    [Export, Shared]
    [ObservableProperty("IsLogin", typeof(bool), IsSetterPublic = false)]
    [ObservableProperty("IsLoggingIn", typeof(bool), IsSetterPublic = false)]
    [ObservableProperty("Username", typeof(string), Initializer = "string.Empty")]
    [ObservableProperty("Password", typeof(string), Initializer = "string.Empty")]
    public partial class EHentaiSettingsVM : ObservableObject
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
