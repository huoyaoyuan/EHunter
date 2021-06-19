using EHunter.Media;
using EHunter.Pixiv.Services.ImageCaching;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Illusts
{
    public class IllustPageVM
    {
        private readonly PixivImageService _imageService;
        private readonly IllustPage _page;

        internal IllustPageVM(IllustVM illust, PixivImageService imageService, IllustPage page)
        {
            Illust = illust;
            _imageService = imageService;
            _page = page;
        }

        public int Index => _page.Index;

        public IllustVM Illust { get; }

        public IImageSource GetImage(IllustSize size)
            => _imageService.GetImage(_page.AtSize(size));
    }
}
