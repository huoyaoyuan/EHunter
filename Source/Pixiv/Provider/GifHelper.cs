using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meowtrix.PixivApi.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace EHunter.Pixiv.Services
{
    public static class GifHelper
    {
        public static async Task ComposeGifAsync(
            AnimatedPictureDetail details,
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            var zipArchive = await details.GetZipArchiveAsync(cancellationToken).ConfigureAwait(false);
            await ComposeGifAsync(zipArchive,
                details.Frames.Select(x => (x.File, x.Delay)),
                stream,
                cancellationToken)
                .ConfigureAwait(false);
        }

        public static async Task ComposeGifAsync(
            ZipArchive zipArchive,
            IEnumerable<(string file, int frameTime)> frames,
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            Image<Rgba32>? image = null;

            foreach (var (filename, frameTime) in frames)
            {
                using (var s = zipArchive.GetEntry(filename)?.Open()
                    ?? throw new InvalidOperationException("Corrupted frame information."))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var frame = await Image.LoadAsync(s).ConfigureAwait(false);
                    image ??= new Image<Rgba32>(frame.Width, frame.Height);
                    image.Frames.AddFrame(frame.Frames[0]);
                }
            }

            if (image is null)
                throw new InvalidOperationException("No frame added to the animated illust.");

            image.Metadata.GetGifMetadata().RepeatCount = 0;
            image.Frames.RemoveFrame(0);
            await image.SaveAsGifAsync(stream, cancellationToken).ConfigureAwait(false);
        }
    }
}
