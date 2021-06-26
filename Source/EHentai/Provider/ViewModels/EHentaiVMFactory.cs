using System.Composition;
using EHunter.EHentai.Api.Models;
using EHunter.Media;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.EHentai.ViewModels
{
    [Export, Shared]
    public class EHentaiVMFactory
    {
        private readonly IMemoryCache _memoryCache;

        [ImportingConstructor]
        public EHentaiVMFactory(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public GalleryVM GreateGallery(Gallery gallery) => new(gallery, this);

        public IImageSource GetThumbnail(Gallery gallery) => new ThumbnailSource(gallery, _memoryCache);
    }
}
