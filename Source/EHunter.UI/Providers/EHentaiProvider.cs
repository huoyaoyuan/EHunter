using System;

namespace EHunter.UI.Providers
{
    public class EHentaiProvider : IProvider
    {
        public string Name => "EHentai";

        public Uri IconUri => new Uri("https://exhentai.org/favicon.ico");
    }
}
