using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Messages
{
    public sealed record InitializationCompleteMessage();

    public sealed record NavigateToIllustMessage(Illust Illust);

    public sealed record NavigateToUserMessage(UserInfo User);

    public sealed record NavigateToTagMessage(Tag Tag);
}
