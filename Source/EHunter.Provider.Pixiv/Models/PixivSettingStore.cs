using Windows.Storage;

#nullable enable
#pragma warning disable CA1508

namespace EHunter.Provider.Pixiv.Models
{
    public class PixivSettingStore
    {
        private readonly ApplicationDataContainer _applicationSetting;
        public PixivSettingStore()
            => _applicationSetting = ApplicationData.Current.LocalSettings
                .CreateContainer("Pixiv", ApplicationDataCreateDisposition.Always);

        public string? RefreshToken
        {
            get => _applicationSetting.Values[nameof(RefreshToken)] as string;
            set => _applicationSetting.Values[nameof(RefreshToken)] = value;
        }

        public bool UseProxy
        {
            get => _applicationSetting.Values[nameof(UseProxy)] as bool? ?? false;
            set => _applicationSetting.Values[nameof(UseProxy)] = value;
        }

        public int MaxDownloadsInParallel
        {
            get => _applicationSetting.Values[nameof(MaxDownloadsInParallel)] as int? ?? 8;
            set => _applicationSetting.Values[nameof(MaxDownloadsInParallel)] = value;
        }
    }
}
