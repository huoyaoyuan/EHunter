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
            .AddTransient<PixivRecentVM>()
            .AddSingleton<UserVMFactory>()
            .AddTransient<OpenedIllustsVM>()
            .AddSingleton<ImageCacheService>()
            .AddTransient<IllustSearchPageVM>();
    }
}
