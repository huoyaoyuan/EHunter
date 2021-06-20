using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Services.ImageCaching;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Services.Images
{
    public abstract class ImageRequest : ICachable
    {
        public abstract object GetCacheKey();

        public abstract Task<Stream> RequestAsync(CancellationToken cancellationToken = default);
    }

    public class PixivImageRequest : ImageRequest
    {
        public ImageInfo ImageInfo { get; }

        public PixivImageRequest(ImageInfo imageInfo) => ImageInfo = imageInfo;

        private record CacheKey(Uri Uri);

        public override object GetCacheKey() => new CacheKey(ImageInfo.Uri);

        public override Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
            => ImageInfo.RequestStreamAsync(cancellationToken);
    }

    public class AnimatedImageRequest : ImageRequest
    {
        public Illust Illust { get; }

        public AnimatedImageRequest(Illust illust) => Illust = illust;

        private record CacheKey(int Id);

        public override object GetCacheKey() => new CacheKey(Illust.Id);

        public override async Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
        {
            var details = await Illust.GetAnimatedDetailAsync(cancellationToken).ConfigureAwait(false);
            var mms = new MemoryStream();
            await GifHelper.ComposeGifAsync(details, mms, cancellationToken).ConfigureAwait(false);

            mms.Seek(0, SeekOrigin.Begin);
            return mms;
        }
    }
}
