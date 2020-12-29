using System;
using System.IO;
using System.Threading.Tasks;
using Meowtrix.PixivApi.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace EHunter.Provider.Pixiv.Services
{
    public static class GifHelper
    {
        public static async Task ComposeGifAsync(AnimatedPictureDetail details,
            Stream stream,
            Action? onPage = null)
        {
            Image<Rgba32>? image = null;

            foreach (var (s, frametime) in await details.ExtractFramesAsync().ConfigureAwait(false))
            {
                using (s)
                {
                    var frame = await Image.LoadAsync(s).ConfigureAwait(false);
                    image ??= new Image<Rgba32>(frame.Width, frame.Height);
                    image.Frames.AddFrame(frame.Frames[0]);

                    onPage?.Invoke();
                }
            }

            if (image is null)
                throw new InvalidOperationException("No frame added to the animated illust.");

            image.Metadata.GetGifMetadata().RepeatCount = 0;
            image.Frames.RemoveFrame(0);
            await image.SaveAsGifAsync(stream).ConfigureAwait(false);
        }
    }
}
