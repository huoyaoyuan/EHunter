using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.Media;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.User
{
    public partial class JumpToUserVM : IllustCollectionVM
    {
        private readonly PixivClient _client;
        private readonly PixivVMFactory _factory;

        [ObservableProperty]
        private int _userId;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(Url))]
        private UserInfo? _userInfo;

        [ObservableProperty]
        private IImageSource? _userAvatar;

        [ObservableProperty]
        private UserDetailInfo? _userDetail;

        public async void JumpToUser()
        {
            try
            {
                IsLoading = true;

                var user = await _client.GetUserDetailAsync(UserId).ConfigureAwait(true);
                UserInfo = user;
                UserDetail = user;
                UserAvatar = _factory.GetImage(user.Avatar);
                Refresh();
            }
            catch
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        public JumpToUserVM(PixivClient client, PixivVMFactory factory, UserInfo? userInfo = null)
            : base(factory)
        {
            _client = client;
            _factory = factory;
            if (userInfo != null)
            {
                UserInfo = userInfo;
                Load(userInfo);
            }

            async void Load(UserInfo user)
            {
                Refresh();
                UserDetail = await user.GetDetailAsync().ConfigureAwait(true);
                UserAvatar = _factory.GetImage(user.Avatar);
            }
        }

        protected override IAsyncEnumerable<Illust>? LoadIllusts() => UserInfo?.GetIllustsAsync();

        public Uri? Url => UserInfo is null ? null
            : new($"https://www.pixiv.net/users/{UserInfo.Id}");
    }
}
