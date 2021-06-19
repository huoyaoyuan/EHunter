using System;
using System.Composition;
using EHunter.Media;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.Services.Images
{
    [Export, Shared]
    public partial class PixivImageService
    {
        private readonly IMemoryCache _memoryCache;

        [ImportingConstructor]
        public PixivImageService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public IImageSource GetImage(ImageInfo imageInfo) => new PixivImageSource(this, imageInfo);

        public IImageSource GetAnimatedIllust(Illust illust)
        {
            if (!illust.IsAnimated)
                throw new ArgumentException("Must be animated illust.", nameof(illust));
            return new PixivAnimatedImageSource(this, illust);
        }
    }
}
