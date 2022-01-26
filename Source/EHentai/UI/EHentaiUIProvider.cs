using EHunter.EHentai.ViewModels;
using EHunter.EHentai.Views;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.EHentai
{
    public class EHentaiUIProvider : EHentaiProvider
    {
        public override Uri IconUri => new("ms-appx:///EHunter.EHentai.UI/Assets/favicon.ico");

        public override object CreateUIRoot(IServiceProvider serviceProvider) => new NavigationPage
        {
            ViewModel = serviceProvider.GetRequiredService<EHentaiNavigationVM>()
        };
    }
}
