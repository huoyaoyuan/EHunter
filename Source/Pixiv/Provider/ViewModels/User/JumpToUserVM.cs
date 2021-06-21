using System;
using System.Collections.Generic;
using EHunter.Media;
using EHunter.Pixiv.ViewModels.Illusts;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.User
{
    [ObservableProperty("UserId", typeof(int))]
    [ObservableProperty("IsLoading", typeof(bool))]
    [ObservableProperty("UserInfo", typeof(UserInfo), IsNullable = true, IsSetterPublic = true)]
    [ObservableProperty("UserAvatar", typeof(IImageSource), IsNullable = true)]
    [ObservableProperty("UserDetail", typeof(UserDetailInfo), IsNullable = true, IsSetterPublic = true)]
    public partial class JumpToUserVM : IllustCollectionVM
    {
        private readonly PixivClient _client;
        private readonly PixivVMFactory _factory;

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

        public JumpToUserVM(PixivClient client, PixivVMFactory factory, IllustVMFactory illustVMFactory, UserInfo? userInfo = null)
            : base(illustVMFactory)
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
