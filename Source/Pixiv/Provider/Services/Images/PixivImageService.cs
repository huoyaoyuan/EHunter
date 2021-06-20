using System.Composition;
using EHunter.Media;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.Services.Images
{
    [Export, Shared]
    public class PixivImageService
    {
        internal readonly IMemoryCache MemoryCache;

        [ImportingConstructor]
        public PixivImageService(IMemoryCache memoryCache) => MemoryCache = memoryCache;

        public IImageSource GetImage(ImageInfo image) => new PixivImageSource(image, this);
        public IImageSource GetAnimatedImage(Illust illust) => new AnimatedImageSource(illust, this);
    }
}
