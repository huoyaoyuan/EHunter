using EHunter.Data;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Data;
using EHunter.Pixiv.Models;
using EHunter.Pixiv.Services;
using EHunter.Pixiv.Services.Download;
using EHunter.Pixiv.ViewModels;
using EHunter.Pixiv.ViewModels.Download;
using EHunter.Services.ImageCaching;
using Meowtrix.PixivApi;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Pixiv
{
    public static class PixivIoc
    {
        public static IServiceCollection ConfigurePixiv(this IServiceCollection services)
            => services
            .ConfigureViewModels()
            .AddSingleton<IPixivSettingStore, PixivSettingStore>()
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
            .AddSingleton<DownloadManager>()
            .AddTransient<IllustVMFactory>();
    }
}
