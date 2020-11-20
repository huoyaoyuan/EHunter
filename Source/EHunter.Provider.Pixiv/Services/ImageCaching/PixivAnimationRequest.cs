using System;
using System.IO;
using System.Threading.Tasks;
using EHunter.Services.ImageCaching;
using Meowtrix.PixivApi.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

#nullable enable

namespace EHunter.Provider.Pixiv.Services.ImageCaching
{
    public sealed class PixivAnimationRequest : ImageRequest
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
            Image<Rgba32>? image = null;

            foreach (var (stream, frametime) in await details.ExtractFramesAsync().ConfigureAwait(false))
            {
                using (stream)
                {
                    var frame = await Image.LoadAsync(stream).ConfigureAwait(false);
                    image ??= new Image<Rgba32>(frame.Width, frame.Height);
                    image.Frames.AddFrame(frame.Frames[0]);
                }
            }

            if (image is null)
                throw new InvalidOperationException("No frame added to the animated illust.");

            image.Metadata.GetGifMetadata().RepeatCount = 0;
            image.Frames.RemoveFrame(0);

            using var mms = new MemoryStream();
            await image.SaveAsGifAsync(mms).ConfigureAwait(false);

            byte[] array = new byte[mms.Length];
            mms.Seek(0, SeekOrigin.Begin);
            mms.Read(array);
            return array;
        }

        public static PixivAnimationRequest? TryCreate(Illust? illust)
            => illust != null ? new(illust) : null;
    }
}
