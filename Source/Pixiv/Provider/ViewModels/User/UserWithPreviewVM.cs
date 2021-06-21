using System.Collections.Generic;
using System.Linq;
using EHunter.Media;
using EHunter.Pixiv.Services.Images;
using EHunter.Pixiv.ViewModels.Illusts;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.User
{
    public class UserWithPreviewVM
    {
        public UserWithPreviewVM(UserInfoWithPreview userInfo, PixivImageService imageService, IllustVMFactory factory)
        {
            UserInfo = userInfo;
            Avatar = imageService.GetImage(userInfo.Avatar);
            PreviewIllusts = userInfo.PreviewIllusts.Select(x => factory.CreateViewModel(x)).ToArray();
        }

        public UserInfoWithPreview UserInfo { get; }

        public IImageSource Avatar { get; }

        public IReadOnlyList<IllustVM> PreviewIllusts { get; }
    }
}
