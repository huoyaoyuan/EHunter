using EHunter.Pixiv.Services.Images;
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

        public ImageVM GetImage(IllustSize size)
            => new(_page.AtSize(size), _imageService);

        public ImageVM Medium => GetImage(IllustSize.Medium);

        public ImageVM Large => GetImage(IllustSize.Large);

        public ImageVM Original => GetImage(IllustSize.Original);
    }
}
