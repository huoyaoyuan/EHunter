using Meowtrix.PixivApi.Models;

namespace EHunter.Provider.Pixiv.Messages
{
    public class NavigateToTagMessage
    {
        public NavigateToTagMessage(Tag tag) => Tag = tag;

        public Tag Tag { get; }
    }
}
