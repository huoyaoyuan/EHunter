using System;
using EHunter.Data;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Data;
using EHunter.Pixiv.Models;
using EHunter.Pixiv.Services;
using EHunter.Pixiv.Services.Download;
using EHunter.Pixiv.ViewModels;
using EHunter.Pixiv.ViewModels.Bookmark;
using EHunter.Pixiv.ViewModels.Download;
using EHunter.Pixiv.ViewModels.Opened;
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
        public abstract Type UIRootType { get; }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<LoginPageVM>()
                .AddTransient<RecentWatchedVM>()
                .AddTransient<UserVMFactory>()
                .AddTransient<OpenedIllustsVM>()
                .AddTransient<IllustSearchManager>()
                .AddTransient<PixivSettingsVM>()
                .AddSingleton<DownloadManager>()
                .AddTransient<IllustVMFactory>()
                .AddTransient<MyBookmarkVM>()

                .AddSingleton<PixivSetting>()
                .AddSingleton<PixivClientService>()
                .AddConversion<ICustomResolver<PixivClient>, PixivClientService>()
                .AddEHunterDbContext<PixivDbContext>()
                .AddSingleton<DownloaderService>();

            ConfigureViewServices(serviceCollection);
        }

        protected virtual void ConfigureViewServices(IServiceCollection serviceCollection) { }
    }
}
