using EHunter.Data;
using EHunter.Data.Pixiv;
using EHunter.DependencyInjection;
using EHunter.Provider.Pixiv.Models;
using EHunter.Provider.Pixiv.Services;
using EHunter.Provider.Pixiv.Services.Download;
using EHunter.Provider.Pixiv.ViewModels;
using EHunter.Provider.Pixiv.ViewModels.Download;
using EHunter.Services.ImageCaching;
using Meowtrix.PixivApi;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Provider.Pixiv
{
    public static class PixivIoc
    {
        public static IServiceCollection ConfigurePixiv(this IServiceCollection services)
            => services
            .ConfigureViewModels()
            .AddSingleton<PixivSettingStore>()
            .AddSingleton<PixivSetting>()
            .AddSingleton<PixivClientService>()
            .AddConversion<ICustomResolver<PixivClient>, PixivClientService>()
            .AddSingleton<ImageCacheService>()
            .AddEHunterDbContext<PixivDbContext>()
            .AddSingleton<DownloaderService>();

        private static IServiceCollection ConfigureViewModels(this IServiceCollection services)
            => services
            .AddTransient<PixivLoginPageVM>()
            .AddTransient<PixivRecentVM>()
            .AddTransient<UserVMFactory>()
            .AddTransient<OpenedIllustsVM>()
            .AddTransient<IllustSearchPageVM>()
            .AddTransient<PixivSettingsVM>()
            .AddSingleton<DownloadManager>();
    }
}
