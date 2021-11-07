using System.Composition;
using EHunter.EHentai.Views;
using EHunter.Providers;

namespace EHunter.EHentai
{
    [Export(typeof(IEHunterProvider))]
    public class EHentaiUIProvider : EHentaiProvider
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public EHentaiUIProvider(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public override Uri IconUri => new("ms-appx:///EHunter.EHentai.UI/Assets/favicon.ico");

        public override object CreateUIRoot() => new NavigationPage
        {
        };
    }
}
