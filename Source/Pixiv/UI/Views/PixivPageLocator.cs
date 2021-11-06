using EHunter.Pixiv.ViewModels;
using EHunter.Pixiv.ViewModels.Bookmark;
using EHunter.Pixiv.ViewModels.Download;
using EHunter.Pixiv.ViewModels.Opened;
using EHunter.Pixiv.ViewModels.Ranking;
using EHunter.Pixiv.ViewModels.Recent;
using EHunter.Pixiv.ViewModels.Search;
using EHunter.Pixiv.ViewModels.User;
using EHunter.Pixiv.Views.Bookmark;
using EHunter.Pixiv.Views.Download;
using EHunter.Pixiv.Views.Opened;
using EHunter.Pixiv.Views.Ranking;
using EHunter.Pixiv.Views.Recent;
using EHunter.Pixiv.Views.Search;
using EHunter.Pixiv.Views.User;

namespace EHunter.Pixiv.Views
{
    internal class PixivPageLocator : IPageLocator
    {
        public Type? MapPageType(object viewModel)
            => viewModel switch
            {
                RecentWatchedVM => typeof(RecentWatchedPage),
                JumpToUserManager => typeof(UsersPage),
                OpenedIllustsVM => typeof(OpenedIllustsPage),
                MyBookmarkVM => typeof(MyBookmarkPage),
                MyFollowingVM => typeof(MyFollowingUsersPage),
                RankingVM => typeof(RankingPage),
                IllustSearchVM => typeof(IllustSearchManager),
                UserSearchVM => typeof(UserSearchPage),
                AllDownloadsVM => typeof(PixivDownloadPage),
                PixivSettingsVM => typeof(PixivSettingsPage),
                _ => null
            };
    }
}
