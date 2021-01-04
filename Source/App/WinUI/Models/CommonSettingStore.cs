using EHunter.Settings;
using Windows.Storage;

#nullable enable
#pragma warning disable CA1508

namespace EHunter.UI.Models
{
    internal class CommonSettingStore : ICommonSettingStore
    {
        private readonly ApplicationDataContainer _applicationSetting;
        public CommonSettingStore() => _applicationSetting = ApplicationData.Current.LocalSettings;

        public string StorageRoot
        {
            get => _applicationSetting.Values[nameof(StorageRoot)] as string ?? string.Empty;
            set => _applicationSetting.Values[nameof(StorageRoot)] = value;
        }

        public string ProxyAddress
        {
            get => _applicationSetting.Values[nameof(ProxyAddress)] as string ?? string.Empty;
            set => _applicationSetting.Values[nameof(ProxyAddress)] = value;
        }

        public int ProxyPort
        {
            get => _applicationSetting.Values[nameof(ProxyPort)] as int? ?? 0;
            set => _applicationSetting.Values[nameof(ProxyPort)] = value;
        }

        public string DbConnectionString
        {
            get => _applicationSetting.Values[nameof(DbConnectionString)] as string ?? string.Empty;
            set => _applicationSetting.Values[nameof(DbConnectionString)] = value;
        }
    }
}
