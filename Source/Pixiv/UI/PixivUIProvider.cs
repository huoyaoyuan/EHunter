using System.Composition;
using EHunter.Pixiv.ViewModels;
using EHunter.Pixiv.Views;
using EHunter.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Pixiv
{
    [Export(typeof(IEHunterProvider))]
    public class PixivUIProvider : PixivProviderBase
    {
        public override Uri IconUri => new("ms-appx:///EHunter.Pixiv.UI/Assets/pixiv.png");

        public override object CreateUIRoot(IServiceProvider serviceProvider) => new PixivRootPage
        {
            ViewModel = serviceProvider.GetRequiredService<PixivRootVM>()
        };
    }
}
