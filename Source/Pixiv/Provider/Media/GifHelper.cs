using System.IO.Compression;
using Meowtrix.PixivApi.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace EHunter.Pixiv.Media
{
    public static class GifHelper
    {
        public static async Task ComposeGifAsync(
            AnimatedPictureDetail details,
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            using var zipArchive = await details.GetZipArchiveAsync(cancellationToken).ConfigureAwait(false);
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

            try
            {
                foreach (var (filename, frameTime) in frames)
                {
                    using (var s = zipArchive.GetEntry(filename)?.Open()
                        ?? throw new InvalidOperationException("Corrupted frame information."))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        using var frameImage = await Image.LoadAsync(s).ConfigureAwait(false);

                        image ??= new Image<Rgba32>(frameImage.Width, frameImage.Height);
                        var frame = frameImage.Frames.RootFrame;
                        frame.Metadata.GetGifMetadata().FrameDelay = frameTime / 10;
                        image.Frames.AddFrame(frame);
                    }
                }

                if (image is null)
                    throw new InvalidOperationException("No frame added to the animated illust.");

                image.Metadata.GetGifMetadata().RepeatCount = 0;
                image.Frames.RemoveFrame(0);
                await image.SaveAsGifAsync(stream, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                image?.Dispose();
            }
        }
    }
}
