using System.Composition;
using EHunter.ComponentModel;

namespace EHunter.EHentai.Settings
{
    [Export, Shared]
    public class EHentaiSetting
    {
        private readonly IEHentaiSettingStore _settingStore;

        [ImportingConstructor]
        public EHentaiSetting(IEHentaiSettingStore settingStore)
        {
            _settingStore = settingStore;
            ConnectionMode = new(_settingStore.ConnectionMode);
        }

        public ObservableProperty<EHentaiConnectionMode> ConnectionMode { get; }

        public void SetConnectionOption(EHentaiConnectionMode value)
        {
            _settingStore.ConnectionMode = value;
            ConnectionMode.Value = value;
        }
    }
}
