using System;
using System.IO;
using System.Threading.Tasks;
using EHunter.Controls;
using EHunter.Pixiv.Services;
using EHunter.Services.ImageCaching;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Controls
{
    public class AnimatedIllust : RichImageBase
    {
        private Illust? _illust;
        public Illust? Illust
        {
            get => _illust;
            set
            {
                if (_illust != value)
                {
                    _illust = value;
                    SetImageEntry(PixivAnimationRequest.TryCreate(value));
                }
            }
        }
    }

    internal sealed class PixivAnimationRequest : ImageRequest
    {
        private readonly Illust _illust;

        public PixivAnimationRequest(Illust illust)
        {
            if (!illust.IsAnimated)
                throw new ArgumentException("The illust must be animated.", nameof(illust));

            _illust = illust;
        }

        private record CacheKey(int IllustId);
        public override object? MemoryCacheKey => new CacheKey(_illust.Id);

        public override async Task<byte[]> GetImageAsync()
        {
            var details = await _illust.GetAnimatedDetailAsync().ConfigureAwait(false);
            var mms = new MemoryStream();
            await GifHelper.ComposeGifAsync(details, mms).ConfigureAwait(false);
            mms.Seek(0, SeekOrigin.Begin);

            byte[] array = new byte[mms.Length];
            mms.Read(array);
            return array;
        }

        public static PixivAnimationRequest? TryCreate(Illust? illust)
            => illust != null ? new(illust) : null;
    }
}
