using System.Composition;
using EHunter.Media;
using EHunter.Pixiv.Media;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.ViewModels
{
    [Export, Shared]
    public class PixivVMFactory
    {
        private readonly IMemoryCache _memoryCache;

        [ImportingConstructor]
        public PixivVMFactory(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public IImageSource GetImage(ImageInfo image) => new PixivImageSource(image, _memoryCache);
        public IImageSource GetAnimatedImage(Illust illust) => new AnimatedImageSource(illust, _memoryCache);
    }
}
