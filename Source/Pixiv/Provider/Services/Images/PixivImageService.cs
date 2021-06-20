using System.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Media;
using EHunter.Services.ImageCaching;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.Services.Images
{
    [Export, Shared]
    public class PixivImageService
    {
        private readonly IMemoryCache _memoryCache;

        [ImportingConstructor]
        public PixivImageService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public IImageSource GetImage(ImageRequest request) => new ImageSource(this, request);

        private class ImageSource : CachedImageSource
        {
            private readonly PixivImageService _owner;
            private readonly ImageRequest _request;

            public ImageSource(PixivImageService owner, ImageRequest request)
                : base(owner._memoryCache)
            {
                _owner = owner;
                _request = request;
            }

            protected override object CreateCacheKey() => _request.GetCacheKey();
            protected override Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
                => _request.RequestAsync(cancellationToken);
        }
    }
}
