using System.Composition;
using EHunter.Media;
using EHunter.Pixiv.Media;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.Services.Images
{
    [Export, Shared]
    public class PixivImageService
    {
        private readonly IMemoryCache _memoryCache;

        [ImportingConstructor]
        public PixivImageService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public IImageSource GetImage(ImageInfo image) => new PixivImageSource(image, _memoryCache);
        public IImageSource GetAnimatedImage(Illust illust) => new AnimatedImageSource(illust, _memoryCache);
    }
}
