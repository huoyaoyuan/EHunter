using System.Collections.Immutable;
using System.Linq;
using EHunter.EHentai.Api.Models;
using EHunter.Media;

namespace EHunter.EHentai.ViewModels
{
    public class GalleryVM
    {
        public Gallery Gallery { get; }

        public GalleryVM(Gallery gallery, EHentaiVMFactory factory)
        {
            Gallery = gallery;
            Thumbnail = factory.GetThumbnail(gallery);
        }

        public IImageSource Thumbnail { get; }

        private static readonly ParsedTitle s_noTitle
            = new(string.Empty, null, null, null, null, null, ImmutableArray<string>.Empty);
        public ParsedTitle DisplayTitle => Gallery.TitleJpn ?? Gallery.Title ?? s_noTitle;

        public GalleryLanguage Language => new(Gallery.Tags.Where(x => x is ("language", _)));
    }
}
