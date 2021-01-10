using System;
using System.Collections.Generic;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels
{
    public class UserVM : IllustCollectionVM
    {
        private readonly PixivClient _client;
        private int _userId;
        public int UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

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

        public UserVM(PixivClient client, IllustVMFactory illustVMFactory, UserInfo? userInfo = null)
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

        private UserInfo? _userInfo;
        public UserInfo? UserInfo
        {
            get => _userInfo;
            private set => SetProperty(ref _userInfo, value);
        }

        private UserDetailInfo? _userDetail;
        public UserDetailInfo? UserDetail
        {
            get => _userDetail;
            private set => SetProperty(ref _userDetail, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        protected override IAsyncEnumerable<Illust>? LoadIllusts() => UserInfo?.GetIllustsAsync();

        public Uri? Url => UserInfo is null ? null
            : new($"https://www.pixiv.net/users/{UserInfo.Id}");
    }

    public class UserVMFactory
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly IllustVMFactory _illustVMFactory;

        public UserVMFactory(ICustomResolver<PixivClient> clientResolver,
            IllustVMFactory illustVMFactory)
        {
            _clientResolver = clientResolver;
            _illustVMFactory = illustVMFactory;
        }

        public UserVM Create() => new(_clientResolver.Resolve(), _illustVMFactory);
        public UserVM Create(UserInfo userInfo) => new(_clientResolver.Resolve(), _illustVMFactory, userInfo);
    }
}
