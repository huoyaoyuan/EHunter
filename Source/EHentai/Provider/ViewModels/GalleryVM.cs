using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EHunter.EHentai.Api.Models;
using EHunter.Media;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.EHentai.ViewModels
{
    public class GalleryVM
    {
        public Gallery Gallery { get; }

        public GalleryVM(Gallery gallery, IMemoryCache memoryCache)
        {
            Gallery = gallery;
            ThumbnailSource = new ThumbnailSource(gallery, memoryCache);
        }

        public IImageSource ThumbnailSource { get; }

        private static readonly ParsedTitle s_noTitle
            = new(string.Empty, null, null, null, null, null, ImmutableArray<string>.Empty);
        public ParsedTitle DisplayTitle => Gallery.TitleJpn ?? Gallery.Title ?? s_noTitle;

        public bool IsTranslated => Gallery.Tags.Any(x => x is ("language", "translated"));

        public string Language => Gallery.Tags.Where(x => x is ("language", not "translated"))
            .Select(x => x.Name)
            .FirstOrDefault() ?? "unknown";
    }

    internal class ThumbnailSource : CachedImageSource
    {
        private readonly Gallery _gallery;

        public ThumbnailSource(Gallery gallery, IMemoryCache memoryCache)
            : base(memoryCache) => _gallery = gallery;

        protected override object CreateCacheKey() => _gallery.Thumbnail;

        protected override Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
            => _gallery.RequestThumbnailAsync();
    }
}
