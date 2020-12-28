﻿using EHunter.DependencyInjection;
using EHunter.Provider.Pixiv.Models;
using Meowtrix.PixivApi;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
    public class PixivSettingsVM : ObservableObject
    {
        private readonly PixivSetting2 _settings;

        public PixivSettingsVM(PixivSetting2 settings, ICustomResolver<PixivClient> clientResolver)
        {
            _settings = settings;

            _useProxy = _settings.UseProxy;
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
    }
}
