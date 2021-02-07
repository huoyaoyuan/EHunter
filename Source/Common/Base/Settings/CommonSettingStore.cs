using System.Composition;
using EHunter.Services;

namespace EHunter.Settings
{
    [Export(typeof(ICommonSettingStore)), Shared]
    public class CommonSettingStore : ICommonSettingStore
    {
        private readonly ISettingsStore _settingsStore;

        [ImportingConstructor]
        public CommonSettingStore(ISettingsStore settingsStore) => _settingsStore = settingsStore;

        public string StorageRoot
        {
            get => _settingsStore.GetStringValue(nameof(StorageRoot)) ?? string.Empty;
            set => _settingsStore.SetStringValue(nameof(StorageRoot), value);
        }

        public string ProxyAddress
        {
            get => _settingsStore.GetStringValue(nameof(ProxyAddress)) ?? string.Empty;
            set => _settingsStore.SetStringValue(nameof(ProxyAddress), value);
        }

        public int ProxyPort
        {
            get => _settingsStore.GetIntValue(nameof(ProxyPort)) ?? 0;
            set => _settingsStore.SetIntValue(nameof(ProxyPort), value);
        }

        public string DbConnectionString
        {
            get => _settingsStore.GetStringValue(nameof(DbConnectionString)) ?? string.Empty;
            set => _settingsStore.SetStringValue(nameof(DbConnectionString), value);
        }
    }
}
