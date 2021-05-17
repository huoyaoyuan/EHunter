using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Settings;
using Meowtrix.PixivApi;

namespace EHunter.Pixiv.ViewModels
{
    [Export]
    public class PixivSettingsVM : ObservableObject
    {
        private readonly PixivSetting _settings;

        [ImportingConstructor]
        public PixivSettingsVM(PixivSetting settings, ICustomResolver<PixivClient> clientResolver)
        {
            _settings = settings;

            _maxDownloadsInParallel = _settings.MaxDownloadsInParallel.Value;
            _connectionMode = _settings.ConnectionMode.Value;
            Client = clientResolver.Resolve();
        }

        public PixivClient Client { get; }

        private int _maxDownloadsInParallel;
        public int MaxDownloadsInParallel
        {
            get => _maxDownloadsInParallel;
            set
            {
                if (SetProperty(ref _maxDownloadsInParallel, value) && value > 0)
                    _settings.SetMaxDownloadsInParallel(value);
            }
        }

        private PixivConnectionMode _connectionMode;
        public PixivConnectionMode ConnectionMode
        {
            get => _connectionMode;
            set
            {
                if (SetProperty(ref _connectionMode, value))
                    _settings.SetConnectionOption(value);
            }
        }
    }
}
