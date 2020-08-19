using System;

namespace EHunter.UI.Providers
{
    public class PixivProvider : IProvider
    {
        public string Name => "Pixiv";

        public Uri IconUri => new Uri("https://www.pixiv.net/favicon.ico");
    }
}
