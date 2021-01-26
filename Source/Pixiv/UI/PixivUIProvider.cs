using System;
using EHunter.Pixiv.Settings;
using EHunter.Pixiv.Views;
using EHunter.Services.ImageCaching;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace EHunter.Pixiv
{
    public class PixivUIProvider : PixivProviderBase
    {
        public override Uri IconUri => new("ms-appx:///EHunter.Pixiv.UI/Assets/pixiv.png");

        public override Type UIRootType => typeof(InitialPage);

        protected override void ConfigureViewServices(IServiceCollection serviceCollection)
            => serviceCollection
                .AddSingleton<IPixivSettingStore, PixivSettingStore>()
                .AddSingleton<ImageCacheService>();
    }
}
