using EHunter.Services;

namespace EHunter.Pixiv.Settings
{
    internal class PixivSettingStore : IPixivSettingStore
    {
        private readonly ISettingsStore _settingsStore;
        public PixivSettingStore(ISettingsStore settingsStore)
            => _settingsStore = settingsStore.GetSubContainer("Pixiv");

        public string? RefreshToken
        {
            get => _settingsStore.GetStringValue(nameof(RefreshToken));
            set => _settingsStore.SetStringValue(nameof(RefreshToken), value);
        }

        public bool UseProxy
        {
            get => _settingsStore.GetBoolValue(nameof(UseProxy)) ?? false;
            set => _settingsStore.SetBoolValue(nameof(UseProxy), value);
        }

        public int MaxDownloadsInParallel
        {
            get => _settingsStore.GetIntValue(nameof(MaxDownloadsInParallel)) ?? 8;
            set => _settingsStore.SetIntValue(nameof(MaxDownloadsInParallel), value);
        }

        public PixivConnectionMode ConnectionMode
        {
            get => (PixivConnectionMode?)_settingsStore.GetIntValue(nameof(ConnectionMode)) ?? PixivConnectionMode.ApplicationProxy;
            set => _settingsStore.SetIntValue(nameof(ConnectionMode), (int)value);
        }
    }
}
