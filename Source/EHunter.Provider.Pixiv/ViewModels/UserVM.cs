﻿using System;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

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
                LoadIllusts();
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
                LoadIllusts();
                UserDetail = await user.GetDetailAsync().ConfigureAwait(true);
            }
        }

        private void LoadIllusts()
        {
            Illusts = UserInfo is null
                ? null
                : new AsyncEnumerableCollection<Illust>(UserInfo.GetIllustsAsync().Age(SelectedAge));

            // TODO: Consider AdvancedCollectionView.Filter
            // Currently doesn't work with mignon/IsR18=true
            // 8.0.0-preview2
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

        private AgeRestriction _selectedAge = AgeRestriction.All;
        public AgeRestriction SelectedAge
        {
            get => _selectedAge;
            set
            {
                if (SetProperty(ref _selectedAge, value))
                {
                    OnPropertyChanged(nameof(IntSelectedAge));
                    LoadIllusts();
                }
            }
        }

        public int IntSelectedAge
        {
            get => (int)SelectedAge;
            set => SelectedAge = (AgeRestriction)value;
        }

        private AsyncEnumerableCollection<Illust>? _illusts;
        public AsyncEnumerableCollection<Illust>? Illusts
        {
            get => _illusts;
            private set => SetProperty(ref _illusts, value);
        }

        public void CopyUrl()
        {
            if (UserInfo is null)
                return;

            var dataPackage = new DataPackage();
            dataPackage.SetText($"https://www.pixiv.net/users/{UserInfo.Id}");
            Clipboard.SetContent(dataPackage);
        }

        public void OpenUrl()
        {
            if (UserInfo is null)
                return;

            _ = Launcher.LaunchUriAsync(new Uri($"https://www.pixiv.net/users/{UserInfo.Id}"));
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
