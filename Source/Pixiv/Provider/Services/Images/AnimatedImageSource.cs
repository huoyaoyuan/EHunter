using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Services.Images
{

    public class AnimatedImageSource : ImageSource
    {
        public Illust Illust { get; }

        public AnimatedImageSource(Illust illust, PixivImageService owner)
            : base(owner) => Illust = illust;

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
