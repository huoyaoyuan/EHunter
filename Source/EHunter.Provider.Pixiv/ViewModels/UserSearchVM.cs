using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
    public class UserSearchVM : ObservableObject
    {
        private UserInfo? _userInfo;
        public UserInfo? UserInfo
        {
            get => _userInfo;
            set => SetProperty(ref _userInfo, value);
        }
    }
}
