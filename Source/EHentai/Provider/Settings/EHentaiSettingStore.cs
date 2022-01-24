using EHunter.Services;

namespace EHunter.EHentai.Settings
{
    internal class EHentaiSettingStore : IEHentaiSettingStore
    {
        private readonly ISettingsStore _settingsStore;
        public EHentaiSettingStore(ISettingsStore settingsStore)
            => _settingsStore = settingsStore.GetSubContainer("EHentai");

        public string? MemberId
        {
            get => _settingsStore.GetStringValue(nameof(MemberId));
            set => _settingsStore.SetStringValue(nameof(MemberId), value);
        }

        public string? PassHash
        {
            get => _settingsStore.GetStringValue(nameof(PassHash));
            set => _settingsStore.SetStringValue(nameof(PassHash), value);
        }

        public EHentaiConnectionMode ConnectionMode
        {
            get => (EHentaiConnectionMode?)_settingsStore.GetIntValue(nameof(ConnectionMode)) ?? EHentaiConnectionMode.ApplicationProxy;
            set => _settingsStore.SetIntValue(nameof(ConnectionMode), (int)value);
        }

        public bool UseExHentai
        {
            get => _settingsStore.GetBoolValue(nameof(UseExHentai)) ?? false;
            set => _settingsStore.SetBoolValue(nameof(UseExHentai), value);
        }
    }
}
