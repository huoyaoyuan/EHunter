using EHunter.Pixiv.Services.Images;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Illusts
{
    public class ImageVM
    {
        private readonly PixivImageService _imageService;
        private readonly ImageRequest _imageRequest;

        public ImageVM(ImageInfo imageInfo, PixivImageService imageService)
        {
            _imageService = imageService;
            _imageRequest = new PixivImageRequest(imageInfo);
        }

        public ImageVM(Illust animateIllust, PixivImageService imageService)
        {
            _imageService = imageService;
            _imageRequest = new AnimatedImageRequest(animateIllust);
        }
    }
}
