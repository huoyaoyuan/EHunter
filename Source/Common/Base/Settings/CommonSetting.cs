using System.Composition;
using System.Net;
using EHunter.ComponentModel;

namespace EHunter.Settings
{
    [Export, Shared, Export(typeof(IStorageSetting)), Export(typeof(IProxySetting)), Export(typeof(IDatabaseSetting))]
    public class CommonSetting : IStorageSetting, IProxySetting, IDatabaseSetting
    {
        private readonly ICommonSettingStore _settingStore;

        [ImportingConstructor]
        public CommonSetting(ICommonSettingStore settingStore)
        {
            _settingStore = settingStore;

            TrySetStorageRoot(settingStore.StorageRoot);
            TrySetProxy(settingStore.ProxyAddress, settingStore.ProxyPort);
            SetConnectionString(settingStore.DbConnectionString);
        }

        public ObservableProperty<string?> StorageRoot { get; } = new(null);
        public bool TrySetStorageRoot(string storageRoot)
        {
            string? newValue = string.IsNullOrEmpty(storageRoot)
                ? null
                : storageRoot;

            _settingStore.StorageRoot = storageRoot;
            StorageRoot.Value = newValue;
            return true;
        }

        public ObservableProperty<IWebProxy?> Proxy { get; } = new(null);
        public bool TrySetProxy(string host, int port)
        {
            WebProxy? newValue = null;
            if (!string.IsNullOrEmpty(host))
            {

                try
                {
                    newValue = new WebProxy(host, port);
                }
                catch
                {
                    return false;
                }
            }

            _settingStore.ProxyAddress = host;
            _settingStore.ProxyPort = port;
            Proxy.Value = newValue;
            return true;
        }

        public ObservableProperty<string> ConnectionString { get; } = new("");
        public void SetConnectionString(string connectionString)
        {
            _settingStore.DbConnectionString = connectionString;
            ConnectionString.Value = connectionString;
        }
    }
}
