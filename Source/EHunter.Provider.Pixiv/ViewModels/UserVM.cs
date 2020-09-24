using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
    public class UserVM : ObservableObject
    {
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

                var client = Ioc.Default.GetRequiredService<PixivSettings>().Client;
                var user = await client.GetUserDetailAsync(UserId).ConfigureAwait(true);
                UserInfo = user;
                UserDetail = user;
            }
            catch
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        public UserVM() { }

        public UserVM(UserInfo userInfo)
        {
            UserInfo = userInfo;
            LoadUserDetail(userInfo);

            async void LoadUserDetail(UserInfo user)
                => UserDetail = await user.GetDetailAsync().ConfigureAwait(true);
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
    }
}
