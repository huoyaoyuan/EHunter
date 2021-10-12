using System.Composition;
using EHunter.Pixiv.Views;
using EHunter.Providers;

namespace EHunter.Pixiv
{
    [Export(typeof(IEHunterProvider))]
    public class PixivUIProvider : PixivProviderBase
    {
        public override Uri IconUri => new("ms-appx:///EHunter.Pixiv.UI/Assets/pixiv.png");

        public override Type UIRootType => typeof(InitialPage);
    }
}
