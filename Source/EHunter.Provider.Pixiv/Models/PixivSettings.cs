using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace EHunter.Provider.Pixiv.Models
{
    public class PixivSettings : ObservableObject
    {
        private readonly ApplicationDataContainer _applicationSetting;

        public PixivSettings()
        {
            _applicationSetting = ApplicationData.Current.LocalSettings;

            _useProxy = (bool?)_applicationSetting.Values[nameof(UseProxy)] ?? false;
        }

        private bool _useProxy;
        public bool UseProxy
        {
            get => _useProxy;
            set
            {
                if (SetProperty(ref _useProxy, value))
                    _applicationSetting.Values[nameof(UseProxy)] = value;
            }
        }
    }
}
