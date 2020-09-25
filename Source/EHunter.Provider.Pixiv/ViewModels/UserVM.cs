﻿using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
    public class UserVM : ObservableObject
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

                Illusts = new AsyncEnumerableCollection<Illust>(user.GetIllustsAsync());
            }
            catch
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        public UserVM(PixivClient client) => _client = client;

        public UserVM(UserInfo userInfo, PixivClient client)
        {
            _client = client;
            UserInfo = userInfo;
            Load(userInfo);

            async void Load(UserInfo user)
            {
                Illusts = new AsyncEnumerableCollection<Illust>(user.GetIllustsAsync());
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

        private AsyncEnumerableCollection<Illust>? _illusts;
        public AsyncEnumerableCollection<Illust>? Illusts
        {
            get => _illusts;
            private set => SetProperty(ref _illusts, value);
        }
    }

    public class UserVMFactory
    {
        private readonly PixivSettings _settings;

        public UserVMFactory(PixivSettings settings) => _settings = settings;

        public UserVM Create() => new UserVM(_settings.Client);
        public UserVM Create(UserInfo userInfo) => new UserVM(userInfo, _settings.Client);
    }
}
