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
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public PixivUIProvider(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public override Uri IconUri => new("ms-appx:///EHunter.Pixiv.UI/Assets/pixiv.png");

        public override object CreateUIRoot() => new InitialPage
        {
            ViewModel = _serviceProvider.GetRequiredService<PixivRootVM>()
        };
    }
}
