using EHunter.Services;
using EHunter.Settings;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.UI.ViewModels
{
    public class CommonSettingVM : ObservableObject
    {
        private readonly CommonSetting _commonSetting;
        private readonly IViewModelService _viewModelService;

        public CommonSettingVM(ICommonSettingStore settingStore, CommonSetting commonSetting, IViewModelService viewModelService)
        {
            _storageRoot = settingStore.StorageRoot;
            _proxyAddress = settingStore.ProxyAddress;
            _proxyPort = settingStore.ProxyPort;
            _dbConnectionString = settingStore.DbConnectionString;
            _commonSetting = commonSetting;
            _viewModelService = viewModelService;
        }

        private string _storageRoot;
        public string StorageRoot
        {
            get => _storageRoot;
            set
            {
                if (SetProperty(ref _storageRoot, value))
                    _commonSetting.TrySetStorageRoot(value);
            }
        }

        private string _proxyAddress;
        public string ProxyAddress
        {
            get => _proxyAddress;
            set
            {
                if (SetProperty(ref _proxyAddress, value))
                    _commonSetting.TrySetProxy(value, _proxyPort);
            }
        }

        private int _proxyPort;
        public int ProxyPort
        {
            get => _proxyPort;
            set
            {
                if (SetProperty(ref _proxyPort, value))
                    _commonSetting.TrySetProxy(_proxyAddress, value);
            }
        }

        private string _dbConnectionString;

        public string DbConnectionString
        {
            get => _dbConnectionString;
            set
            {
                if (SetProperty(ref _dbConnectionString, value))
                    _commonSetting.SetConnectionString(value);
            }
        }

        public async void BrowseStorageRoot()
        {
            string? folder = await _viewModelService.BrowseFolderAsync().ConfigureAwait(true);
            if (folder != null)
                StorageRoot = folder;
        }
    }
}
