using Meowtrix.PixivApi.Models;

namespace EHunter.Provider.Pixiv.Messages
{
    public class NavigateToIllustMessage
    {
        public NavigateToIllustMessage(Illust illust) => Illust = illust;

        public Illust Illust { get; }
    }
}
