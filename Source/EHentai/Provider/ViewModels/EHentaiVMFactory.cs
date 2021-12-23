using System.Composition;
using EHunter.DependencyInjection;
using EHunter.EHentai.Api;
using EHunter.EHentai.Api.Models;
using EHunter.Media;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.EHentai.ViewModels
{
    [Export, Shared]
    public class EHentaiVMFactory
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICustomResolver<EHentaiClient> _clientResolver;

        [ImportingConstructor]
        public EHentaiVMFactory(
            IMemoryCache memoryCache,
            ICustomResolver<EHentaiClient> clientResolver)
        {
            _memoryCache = memoryCache;
            _clientResolver = clientResolver;
        }

        public GalleryVM GreateGallery(Gallery gallery) => new(gallery, this);

        public IImageSource GetThumbnail(Gallery gallery) => new ThumbnailSource(_clientResolver.Resolve(), gallery, _memoryCache);
    }
}
