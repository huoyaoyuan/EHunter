﻿using System.Windows.Input;
using EHunter.Media;
using EHunter.Pixiv.ViewModels.Illusts;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.User
{
    public class UserWithPreviewVM
    {
        public UserWithPreviewVM(UserInfoWithPreview userInfo, PixivVMFactory factory)
        {
            UserInfo = userInfo;
            Avatar = factory.GetImage(userInfo.Avatar);
            PreviewIllusts = userInfo.PreviewIllusts.Select(x => factory.CreateViewModel(x)).ToArray();
            NavigateToUserCommand = factory.NavigateToUserCommand;
        }

        public UserInfoWithPreview UserInfo { get; }

        public IImageSource Avatar { get; }

        public IReadOnlyList<IllustVM> PreviewIllusts { get; }

        public ICommand NavigateToUserCommand { get; }
    }
}
