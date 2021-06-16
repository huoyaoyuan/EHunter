using System.Collections.Immutable;
using System.Linq;
using EHunter.EHentai.Api.Models;

namespace EHunter.EHentai.ViewModels
{
    public class GalleryVM
    {
        public Gallery Gallery { get; }

        public GalleryVM(Gallery gallery) => Gallery = gallery;

        private static readonly ParsedTitle s_noTitle
            = new(string.Empty, null, null, null, null, null, ImmutableArray<string>.Empty);
        public ParsedTitle DisplayTitle => Gallery.TitleJpn ?? Gallery.Title ?? s_noTitle;

        public bool IsTranslated => Gallery.Tags.Any(x => x is ("language", "translated"));

        public string Language => Gallery.Tags.Where(x => x is ("language", not "translated"))
            .Select(x => x.Name)
            .FirstOrDefault() ?? "unknown";
    }
}
