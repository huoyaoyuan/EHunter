using System.IO;
using System.Net;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Settings
{
    public class CommonSetting : IStorageSetting, IProxySetting, IDatabaseSetting
    {
        private readonly ICommonSettingStore _settingStore;

        public CommonSetting(ICommonSettingStore settingStore)
        {
            _settingStore = settingStore;

            TrySetStorageRoot(settingStore.StorageRoot);
            TrySetProxy(settingStore.ProxyAddress, settingStore.ProxyPort);
            SetConnectionString(settingStore.DbConnectionString);
        }

        public ObservableProperty<DirectoryInfo?> StorageRoot { get; } = new(null);
        public bool TrySetStorageRoot(string storageRoot)
        {
            DirectoryInfo? newValue;
            try
            {
                newValue = string.IsNullOrEmpty(storageRoot)
                    ? null
                    : new DirectoryInfo(storageRoot);
            }
            catch
            {
                return false;
            }

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

    public static class CommonSettingDependencyInjectionExtensions
    {
        public static IServiceCollection AddCommonSettings(this IServiceCollection serviceCollection)
            => serviceCollection
                .AddSingleton<CommonSetting>()
                .AddConversion<IStorageSetting, CommonSetting>()
                .AddConversion<IProxySetting, CommonSetting>()
                .AddConversion<IDatabaseSetting, CommonSetting>();
    }
}
