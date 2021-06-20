using EHunter.Media;
using EHunter.Pixiv.Services.Images;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Illusts
{
    public class ImageVM
    {
        private readonly IImageSource _imageRequest;

        public ImageVM(ImageInfo imageInfo, PixivImageService imageService)
        {
            _imageRequest = new PixivImageSource(imageInfo, imageService);
        }

        public ImageVM(Illust animateIllust, PixivImageService imageService)
        {
            _imageRequest = new AnimatedImageSource(animateIllust, imageService);
        }
    }
}
