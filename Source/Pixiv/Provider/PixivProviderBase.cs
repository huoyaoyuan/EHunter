using EHunter.Data;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Data;
using EHunter.Pixiv.Services;
using EHunter.Pixiv.Services.Download;
using EHunter.Pixiv.Settings;
using EHunter.Pixiv.ViewModels;
using EHunter.Pixiv.ViewModels.Bookmark;
using EHunter.Pixiv.ViewModels.Download;
using EHunter.Pixiv.ViewModels.Opened;
using EHunter.Pixiv.ViewModels.Ranking;
using EHunter.Pixiv.ViewModels.Recent;
using EHunter.Pixiv.ViewModels.Search;
using EHunter.Pixiv.ViewModels.User;
using EHunter.Providers;
using Meowtrix.PixivApi;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Pixiv
{
    public abstract class PixivProviderBase : IEHunterProvider
    {
        public string Title => "Pixiv";
        public abstract Uri IconUri { get; }
        public abstract object CreateUIRoot(IServiceProvider serviceProvider);

        public virtual void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<PixivSetting>()
                .AddSingleton<IPixivSettingStore, PixivSettingStore>()
                .AddSingleton<PixivRootVM>()
                .AddScoped<ICustomResolver<PixivClient>, PixivClientService>()
                .AddTransient<PixivSettingsVM>()
                .AddTransient<LoginPageVM>()
                .AddScoped<DownloaderService>()
                .AddScoped<DownloadManager>()
                .AddScoped<PixivVMFactory>()

                .AddSingleton<EHunterDbContextResolver<EHunterDbContext>>()
                .AddSingleton<IDbContextFactoryResolver<EHunterDbContext>>(sp => sp.GetRequiredService<EHunterDbContextResolver<EHunterDbContext>>())

                .AddSingleton<EHunterDbContextResolver<PixivDbContext>>()
                .AddSingleton<IDbContextFactoryResolver<PixivDbContext>>(sp => sp.GetRequiredService<EHunterDbContextResolver<PixivDbContext>>())

                .AddTransient<MyBookmarkVM>()
                .AddTransient<MyFollowingVM>()
                .AddScoped<AllDownloadsVM>()
                .AddTransient<OpenedIllustsVM>()
                .AddTransient<RankingVM>()
                .AddTransient<RecentWatchedVM>()
                .AddScoped<IllustSearchManager>()
                .AddTransient<UserSearchVM>()
                .AddTransient<JumpToUserManager>();
        }
    }
}
