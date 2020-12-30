using EHunter.DependencyInjection;
using EHunter.Provider.Pixiv.Models;
using Meowtrix.PixivApi;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
    public class PixivSettingsVM : ObservableObject
    {
        private readonly PixivSetting _settings;

        public PixivSettingsVM(PixivSetting settings, ICustomResolver<PixivClient> clientResolver)
        {
            _settings = settings;

            _useProxy = _settings.UseProxy;
            _maxDownloadsInParallel = _settings.MaxDownloadsInParallel;
            Client = clientResolver.Resolve();
        }

        public PixivClient Client { get; }

        private bool _useProxy;
        public bool UseProxy
        {
            get => _useProxy;
            set
            {
                if (SetProperty(ref _useProxy, value))
                    _settings.SetUseProxy(value);
            }
        }

        private int _maxDownloadsInParallel;
        public int MaxDownloadsInParallel
        {
            get => _maxDownloadsInParallel;
            set
            {
                if (SetProperty(ref _maxDownloadsInParallel, value))
                    _settings.SetMaxDownloadsInParallel(value);
            }
        }
    }
}
