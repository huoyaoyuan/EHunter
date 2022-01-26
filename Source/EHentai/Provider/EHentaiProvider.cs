using EHunter.DependencyInjection;
using EHunter.EHentai.Api;
using EHunter.EHentai.Services;
using EHunter.EHentai.Settings;
using EHunter.EHentai.ViewModels;
using EHunter.EHentai.ViewModels.GalleryList;
using EHunter.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.EHentai
{
    public abstract class EHentaiProvider : IEHunterProvider
    {
        public string Title => "EHentai";
        public abstract Uri IconUri { get; }
        public abstract object CreateUIRoot(IServiceProvider serviceProvider);
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<EHentaiSetting>()
                .AddSingleton<IEHentaiSettingStore, EHentaiSettingStore>()
                .AddSingleton<ICustomResolver<EHentaiClient>, EHentaiClientService>()
                .AddSingleton<EHentaiNavigationVM>()
                .AddTransient<EHentaiSettingsVM>()
                .AddSingleton<EHentaiVMFactory>()

                .AddTransient<GalleryListManager>();
        }
    }
}
