using System;
using Meowtrix.PixivApi.Models;

namespace EHunter.Provider.Pixiv.Messages
{
    internal sealed record LoginFailedMessage(Exception Exception);

    internal sealed record InitializationCompleteMessage();

    internal sealed record NavigateToIllustMessage(Illust Illust);

    internal sealed record NavigateToUserMessage(UserInfo User);

    internal sealed record NavigateToTagMessage(Tag Tag);
}
