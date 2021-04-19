using System;
using System.Composition;
using EHunter.EHentai;
using EHunter.Pixiv.Views;
using EHunter.Providers;

#nullable enable

namespace EHunter.Pixiv
{
    [Export(typeof(IEHunterProvider))]
    public class EHentaiUIProvider : EHentaiProvider
    {
        public override Uri IconUri => new("ms-appx:///EHunter.EHentai.UI/Assets/favicon.ico");
        public override Type UIRootType => typeof(NavigationPage);
    }
}
