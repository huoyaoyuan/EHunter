using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Services.Images
{
    public class PixivImageSource : ImageSource
    {
        public ImageInfo ImageInfo { get; }

        public PixivImageSource(ImageInfo imageInfo, PixivImageService owner)
            : base(owner) => ImageInfo = imageInfo;

        private record CacheKey(Uri Uri);

        protected override object CreateCacheKey() => new CacheKey(ImageInfo.Uri);

        protected override Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
            => ImageInfo.RequestStreamAsync(cancellationToken);
    }
}
