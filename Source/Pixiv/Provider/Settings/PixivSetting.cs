using EHunter.ComponentModel;

namespace EHunter.Pixiv.Settings
{
    public class PixivSetting
    {
        private readonly IPixivSettingStore _settingStore;

        public PixivSetting(IPixivSettingStore settingStore)
        {
            _settingStore = settingStore;
            MaxDownloadsInParallel = new(_settingStore.MaxDownloadsInParallel);
            ConnectionMode = new(_settingStore.ConnectionMode);
        }

        public ObservableProperty<int> MaxDownloadsInParallel { get; }

        public void SetMaxDownloadsInParallel(int value)
        {
            _settingStore.MaxDownloadsInParallel = value;
            MaxDownloadsInParallel.Value = value;
        }

        public ObservableProperty<PixivConnectionMode> ConnectionMode { get; }

        public void SetConnectionOption(PixivConnectionMode value)
        {
            _settingStore.ConnectionMode = value;
            ConnectionMode.Value = value;
        }
    }
}
