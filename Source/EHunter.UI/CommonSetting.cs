using System.IO;
using System.Net;
using System.Net.Http;
using EHunter.Settings;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace EHunter.UI
{
    public class CommonSetting : ObservableObject, ICommonSetting
    {
        private readonly ApplicationDataContainer _applicationSetting;

        public CommonSetting()
        {
            _applicationSetting = ApplicationData.Current.LocalSettings;

            _storageRoot = (string?)_applicationSetting.Values[nameof(StorageRoot)] ?? string.Empty;
            _proxyAddress = (string?)_applicationSetting.Values[nameof(ProxyAddress)] ?? string.Empty;
            _proxyPort = (int?)_applicationSetting.Values[nameof(ProxyPort)] ?? 0;
            _dbConnectionString = (string?)_applicationSetting.Values[nameof(DbConnectionString)] ?? string.Empty;
        }

        private string _storageRoot;
        public string StorageRoot
        {
            get => _storageRoot;
            set
            {
                if (SetProperty(ref _storageRoot, value))
                    _applicationSetting.Values[nameof(StorageRoot)] = value;
            }
        }

        public DirectoryInfo StorageRootDirectory => new DirectoryInfo(StorageRoot);

        private string _proxyAddress;
        public string ProxyAddress
        {
            get => _proxyAddress;
            set
            {
                if (SetProperty(ref _proxyAddress, value))
                {
                    _applicationSetting.Values[nameof(ProxyAddress)] = value;
                    UpdateProxy();
                }
            }
        }

        private int _proxyPort;
        public int ProxyPort
        {
            get => _proxyPort;
            set
            {
                if (SetProperty(ref _proxyPort, value))
                {
                    _applicationSetting.Values[nameof(ProxyPort)] = value;
                    UpdateProxy();
                }
            }
        }

        private void UpdateProxy() => HttpClient.DefaultProxy = new WebProxy(ProxyAddress, ProxyPort);

        public IWebProxy Proxy => new WebProxy(ProxyAddress, ProxyPort);

        private string _dbConnectionString;
        public string DbConnectionString
        {
            get => _dbConnectionString;
            set
            {
                if (SetProperty(ref _dbConnectionString, value))
                    _applicationSetting.Values[nameof(DbConnectionString)] = value;
            }
        }
    }
}
