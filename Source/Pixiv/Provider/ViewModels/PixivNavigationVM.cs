using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EHunter.Pixiv.Messages;
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
    public sealed partial class PixivNavigationVM : ObservableObject,
        IRecipient<NavigateToUserMessage>,
        IRecipient<NavigateToIllustMessage>,
        IRecipient<NavigateToTagMessage>
    {
        public PixivNavigationVM(IServiceProvider serviceProvider, IMessenger messenger)
        {
            Recent = serviceProvider.GetRequiredService<RecentWatchedVM>();
            Users = serviceProvider.GetRequiredService<JumpToUserManager>();
            Opened = serviceProvider.GetRequiredService<OpenedIllustsVM>();
            Bookmark = serviceProvider.GetRequiredService<MyBookmarkVM>();
            Following = serviceProvider.GetRequiredService<MyFollowingVM>();
            Ranking = serviceProvider.GetRequiredService<RankingVM>();
            IllustSearch = serviceProvider.GetRequiredService<IllustSearchManager>();
            UserSearch = serviceProvider.GetRequiredService<UserSearchVM>();
            Downloads = serviceProvider.GetRequiredService<AllDownloadsVM>();
            Settings = serviceProvider.GetRequiredService<PixivSettingsVM>();

            SelectedPage = Recent;

            messenger.Register(Users);
            messenger.Register(Opened);
            messenger.Register(IllustSearch);

            messenger.RegisterAll(this);
        }

        public RecentWatchedVM Recent { get; }
        public JumpToUserManager Users { get; }
        public OpenedIllustsVM Opened { get; }
        public MyBookmarkVM Bookmark { get; }
        public MyFollowingVM Following { get; }
        public RankingVM Ranking { get; }
        public IllustSearchManager IllustSearch { get; }
        public UserSearchVM UserSearch { get; }
        public AllDownloadsVM Downloads { get; }

        public PixivSettingsVM Settings { get; }

        [ObservableProperty]
        private object? _selectedPage;

        void IRecipient<NavigateToUserMessage>.Receive(NavigateToUserMessage message) => SelectedPage = Users;
        void IRecipient<NavigateToIllustMessage>.Receive(NavigateToIllustMessage message) => SelectedPage = Opened;
        void IRecipient<NavigateToTagMessage>.Receive(NavigateToTagMessage message) => SelectedPage = IllustSearch;
    }
}
