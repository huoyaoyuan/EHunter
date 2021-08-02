using System.Composition;
using EHunter.Pixiv.ViewModels;
using EHunter.Pixiv.Views;
using EHunter.Providers;

namespace EHunter.Pixiv
{
    [Export(typeof(IEHunterProvider))]
    public class PixivUIProvider : PixivProviderBase
    {
        public override Uri IconUri => new("ms-appx:///EHunter.Pixiv.UI/Assets/pixiv.png");

        public override object CreateUIRoot() => new InitialPage
        {
            ViewModel = new PixivRootVM()
        };
    }
}
