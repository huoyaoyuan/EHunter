using System.Collections.Generic;
using System.Linq;
using EHunter.Media;
using EHunter.Pixiv.ViewModels.Illusts;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.User
{
    public class UserWithPreviewVM
    {
        public UserWithPreviewVM(UserInfoWithPreview userInfo, PixivVMFactory vmFactory, IllustVMFactory factory)
        {
            UserInfo = userInfo;
            Avatar = vmFactory.GetImage(userInfo.Avatar);
            PreviewIllusts = userInfo.PreviewIllusts.Select(x => factory.CreateViewModel(x)).ToArray();
        }

        public UserInfoWithPreview UserInfo { get; }

        public IImageSource Avatar { get; }

        public IReadOnlyList<IllustVM> PreviewIllusts { get; }
    }
}
