using CommunityToolkit.Mvvm.Messaging;
using EHunter.Providers;
using EHunter.Services;
using EHunter.Settings;
using EHunter.UI.Services;
using EHunter.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.UI
{
    public static class EHunterApp
    {
        public static IServiceProvider Build(params IEHunterProvider[] providers)
        {
            var services = new ServiceCollection()
                .AddMemoryCache(o =>
                {
                    o.SizeLimit = 2 * (1L << 30);
                    o.CompactionPercentage = 0.9;
                })
                .AddSingleton<IMessenger, WeakReferenceMessenger>()
                .AddSingleton<IViewModelService, ViewModelService>()
                .AddSingleton<ISettingsStore, WinRTSettingsStore>()
                .AddSingleton<CommonSetting>()
                .AddSingleton<IStorageSetting>(sp => sp.GetRequiredService<CommonSetting>())
                .AddSingleton<IDatabaseSetting>(sp => sp.GetRequiredService<CommonSetting>())
                .AddSingleton<IProxySetting>(sp => sp.GetRequiredService<CommonSetting>())
                .AddSingleton<ICommonSettingStore, CommonSettingStore>()
                .AddSingleton<MainWindowVM>()
                .AddTransient<CommonSettingVM>();

            foreach (var provider in providers)
            {
                services.AddSingleton(provider);
                provider.ConfigureServices(services);
            }

            return services.BuildServiceProvider();
        }
    }
}
