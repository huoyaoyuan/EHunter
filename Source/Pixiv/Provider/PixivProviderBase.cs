using EHunter.Providers;

namespace EHunter.Pixiv
{
    public abstract class PixivProviderBase : IEHunterProvider
    {
        public string Title => "Pixiv";
        public abstract Uri IconUri { get; }
        public abstract object CreateUIRoot();
    }
}
