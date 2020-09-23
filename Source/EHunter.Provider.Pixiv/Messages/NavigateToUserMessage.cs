using Meowtrix.PixivApi.Models;

namespace EHunter.Provider.Pixiv.Messages
{
    public sealed class NavigateToUserMessage
    {
        public NavigateToUserMessage(UserInfo user) => User = user;

        public UserInfo User { get; }
    }
}
