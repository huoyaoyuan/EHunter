using EHunter.DependencyInjection;
using EHunter.Provider.Pixiv.Models;
using EHunter.Provider.Pixiv.Services;
using EHunter.Provider.Pixiv.ViewModels;
using EHunter.Services.ImageCaching;
using Meowtrix.PixivApi;
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
            .AddSingleton<PixivClientService>()
            .AddConversion<ICustomResolver<PixivClient>, PixivClientService>()
            .AddTransient<PixivLoginPageVM>()
            .AddSingleton<PixivRecentVM>()
            .AddSingleton<UserVMFactory>()
            .AddSingleton<OpenedIllustsVM>()
            .AddSingleton<ImageCacheService>()
            .AddSingleton<IllustSearchPageVM>()
            .AddSingleton<DatabaseAccessor>();
    }
}
