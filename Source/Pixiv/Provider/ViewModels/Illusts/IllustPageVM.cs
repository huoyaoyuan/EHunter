using EHunter.Media;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Illusts
{
    public class IllustPageVM
    {
        private readonly PixivVMFactory _factory;
        private readonly IllustPage _page;

        internal IllustPageVM(IllustVM illust, PixivVMFactory factory, IllustPage page)
        {
            Illust = illust;
            _factory = factory;
            _page = page;
        }

        public int Index => _page.Index;

        public IllustVM Illust { get; }

        public IImageSource GetImage(IllustSize size) => _factory.GetImage(_page.AtSize(size));

        public IImageSource Medium => GetImage(IllustSize.Medium);

        public IImageSource Large => GetImage(IllustSize.Large);

        public IImageSource Original => GetImage(IllustSize.Original);
    }
}
