using EHunter.ComponentModel;

namespace EHunter.EHentai.Settings
{
    public class EHentaiSetting
    {
        private readonly IEHentaiSettingStore _settingStore;

        public EHentaiSetting(IEHentaiSettingStore settingStore)
        {
            _settingStore = settingStore;
            ConnectionMode = new(_settingStore.ConnectionMode);
            UseExHentai = new(_settingStore.UseExHentai);
        }

        public ObservableProperty<EHentaiConnectionMode> ConnectionMode { get; }

        public void SetConnectionOption(EHentaiConnectionMode value)
        {
            _settingStore.ConnectionMode = value;
            ConnectionMode.Value = value;
        }

        public ObservableProperty<bool> UseExHentai { get; }

        public void SetUseExHentai(bool value)
        {
            _settingStore.UseExHentai = value;
            UseExHentai.Value = value;
        }
    }
}
