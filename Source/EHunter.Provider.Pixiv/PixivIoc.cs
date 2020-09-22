using EHunter.Provider.Pixiv.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Provider.Pixiv
{
    public static class PixivIoc
    {
        public static IServiceCollection ConfigurePixiv(this IServiceCollection services)
            => services
            .AddSingleton<PixivSettings>()
            .AddTransient<PixivRecentVM>();
    }
}
