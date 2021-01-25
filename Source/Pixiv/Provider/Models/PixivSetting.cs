using EHunter.ComponentModel;

namespace EHunter.Pixiv.Models
{
    public class PixivSetting
    {
        private readonly IPixivSettingStore _settingStore;

        public PixivSetting(IPixivSettingStore settingStore)
        {
            _settingStore = settingStore;
            UseProxy = new ObservableProperty<bool>(_settingStore.UseProxy);
            MaxDownloadsInParallel = new ObservableProperty<int>(_settingStore.MaxDownloadsInParallel);
        }

        public ObservableProperty<bool> UseProxy { get; }

        public void SetUseProxy(bool value)
        {
            UseProxy.Value = value;
            _settingStore.UseProxy = value;
        }

        public ObservableProperty<int> MaxDownloadsInParallel { get; }

        public void SetMaxDownloadsInParallel(int value)
        {
            _settingStore.MaxDownloadsInParallel = value;
            MaxDownloadsInParallel.Value = value;
        }
    }
}
