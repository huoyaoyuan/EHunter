using EHunter.Provider.Pixiv.Models;
using EHunter.Provider.Pixiv.Services;
using EHunter.Provider.Pixiv.ViewModels;
using EHunter.Services.ImageCaching;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Provider.Pixiv
{
    public static class PixivIoc
    {
        public static IServiceCollection ConfigurePixiv(this IServiceCollection services)
            => services
            .AddSingleton<PixivSettings>()
            .AddSingleton<PixivSettingStore>()
            .AddSingleton<PixivSetting2>()
            .AddSingleton<PixivRecentVM>()
            .AddSingleton<UserVMFactory>()
            .AddSingleton<OpenedIllustsVM>()
            .AddSingleton<ImageCacheService>()
            .AddSingleton<IllustSearchPageVM>()
            .AddSingleton<DatabaseAccessor>();
    }
}
