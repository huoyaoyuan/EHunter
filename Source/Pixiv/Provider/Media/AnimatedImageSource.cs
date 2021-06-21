using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Media;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.Media
{

    internal class AnimatedImageSource : CachedImageSource
    {
        public Illust Illust { get; }

        internal AnimatedImageSource(Illust illust, IMemoryCache memoryCache)
            : base(memoryCache) => Illust = illust;

        private record CacheKey(int Id);

        protected override object CreateCacheKey() => new CacheKey(Illust.Id);

        protected override async Task<Stream> RequestAsync(CancellationToken cancellationToken = default)
        {
            var details = await Illust.GetAnimatedDetailAsync(cancellationToken).ConfigureAwait(false);
            var mms = new MemoryStream();
            await GifHelper.ComposeGifAsync(details, mms, cancellationToken).ConfigureAwait(false);

            mms.Seek(0, SeekOrigin.Begin);
            return mms;
        }
    }
}
