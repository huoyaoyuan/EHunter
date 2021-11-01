using System.Composition;
using EHunter.EHentai.Views;
using EHunter.Providers;

namespace EHunter.EHentai
{
    [Export(typeof(IEHunterProvider))]
    public class EHentaiUIProvider : EHentaiProvider
    {
        public override Uri IconUri => new("ms-appx:///EHunter.EHentai.UI/Assets/favicon.ico");
        public override Type UIRootType => typeof(NavigationPage);
    }
}
