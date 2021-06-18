using System;
using System.Collections.Generic;
using EHunter.Pixiv.ViewModels.Illusts;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.User
{
    [ObservableProperty("UserId", typeof(int))]
    [ObservableProperty("IsLoading", typeof(bool))]
    [ObservableProperty("UserInfo", typeof(UserInfo), IsNullable = true, IsSetterPublic = true)]
    [ObservableProperty("UserDetail", typeof(UserDetailInfo), IsNullable = true, IsSetterPublic = true)]
    public partial class JumpToUserVM : IllustCollectionVM
    {
        private readonly PixivClient _client;

        public async void JumpToUser()
        {
            try
            {
                IsLoading = true;

                var user = await _client.GetUserDetailAsync(UserId).ConfigureAwait(true);
                UserInfo = user;
                UserDetail = user;
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

        public JumpToUserVM(PixivClient client, IllustVMFactory illustVMFactory, UserInfo? userInfo = null)
            : base(illustVMFactory)
        {
            _client = client;

            if (userInfo != null)
            {
                UserInfo = userInfo;
                Load(userInfo);
            }

            async void Load(UserInfo user)
            {
                Refresh();
                UserDetail = await user.GetDetailAsync().ConfigureAwait(true);
            }
        }

        protected override IAsyncEnumerable<Illust>? LoadIllusts() => UserInfo?.GetIllustsAsync();

        public Uri? Url => UserInfo is null ? null
            : new($"https://www.pixiv.net/users/{UserInfo.Id}");
    }
}
