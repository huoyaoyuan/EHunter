using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.Pixiv.ViewModels.Bookmark;
using EHunter.Pixiv.ViewModels.Download;
using EHunter.Pixiv.ViewModels.Opened;
using EHunter.Pixiv.ViewModels.Ranking;
using EHunter.Pixiv.ViewModels.Recent;
using EHunter.Pixiv.ViewModels.Search;
using EHunter.Pixiv.ViewModels.User;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Pixiv.ViewModels
{
    [Export]
    public class PixivRootVM : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public PixivRootVM(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public LoginPageVM Login => _serviceProvider.GetRequiredService<LoginPageVM>();
        public RecentWatchedVM Recent => _serviceProvider.GetRequiredService<RecentWatchedVM>();
        public JumpToUserManager Users => _serviceProvider.GetRequiredService<JumpToUserManager>();
        public OpenedIllustsVM Opened => _serviceProvider.GetRequiredService<OpenedIllustsVM>();
        public MyBookmarkVM Bookmark => _serviceProvider.GetRequiredService<MyBookmarkVM>();
        public MyFollowingVM Following => _serviceProvider.GetRequiredService<MyFollowingVM>();
        public RankingVM Ranking => _serviceProvider.GetRequiredService<RankingVM>();
        public IllustSearchManager IllustSearch => _serviceProvider.GetRequiredService<IllustSearchManager>();
        public UserSearchVM UserSearch => _serviceProvider.GetRequiredService<UserSearchVM>();
        public AllDownloadsVM Downloads => _serviceProvider.GetRequiredService<AllDownloadsVM>();
    }
}
