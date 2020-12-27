using System;
using System.IO;
using System.Net;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Settings;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace EHunter.UI.Models
{
    public class CommonSetting2 : IStorageSetting, IProxySetting, IDatabaseSetting
    {
        public CommonSetting2(ICommonSettingStore settingStore)
        {
            _settingStore = settingStore;

            TrySetStorageRoot(settingStore.StorageRoot);
            TrySetProxy(settingStore.ProxyAddress, settingStore.ProxyPort);
            SetConnectionString(settingStore.DbConnectionString);
        }

        private readonly ObservableProperty<DirectoryInfo?> _storageRoot
            = new(null);
        public DirectoryInfo? StorageRoot => _storageRoot.Value;
        public IObservable<DirectoryInfo?> StorageChanged => _storageRoot.ValueObservable;
        public bool TrySetStorageRoot(string storageRoot)
        {
            DirectoryInfo? newValue;
            try
            {
                newValue = string.IsNullOrEmpty(storageRoot)
                    ? new DirectoryInfo(storageRoot)
                    : null;
            }
            catch
            {
                return false;
            }

            _settingStore.StorageRoot = storageRoot;
            _storageRoot.Value = newValue;
            return true;
        }

        private readonly ObservableProperty<IWebProxy?> _proxy
            = new(null);
        public IWebProxy? WebProxy => _proxy.Value;
        public IObservable<IWebProxy?> ProxyChanged => _proxy.ValueObservable;
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
            _proxy.Value = newValue;
            return true;
        }

        private readonly ObservableProperty<string> _connectionString
            = new("");
        private readonly ICommonSettingStore _settingStore;

        public string ConnectionString => _connectionString.Value;
        public IObservable<string> ConnectionStringChanged => _connectionString.ValueObservable;
        public void SetConnectionString(string connectionString)
        {
            _settingStore.DbConnectionString = connectionString;
            _connectionString.Value = connectionString;
        }
    }

    public static class CommonSettingDependencyInjectionExtensions
    {
        public static IServiceCollection AddCommonSettings(this IServiceCollection serviceCollection)
            => serviceCollection
                .AddSingleton<CommonSetting2>()
                .AddConversion<IStorageSetting, CommonSetting2>()
                .AddConversion<IProxySetting, CommonSetting2>()
                .AddConversion<IDatabaseSetting, CommonSetting2>();
    }
}
