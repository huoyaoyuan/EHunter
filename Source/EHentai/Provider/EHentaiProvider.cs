using EHunter.Providers;

namespace EHunter.EHentai
{
    public abstract class EHentaiProvider : IEHunterProvider
    {
        public string Title => "EHentai";
        public abstract Uri IconUri { get; }
        public abstract Type UIRootType { get; }
    }
}
