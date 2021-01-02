using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using EHunter.Data;
using EHunter.Data.Pixiv;
using Meowtrix.PixivApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Provider.Pixiv.Services.Download
{
    public class AnimatedDownloadTask : DownloadTask
    {
        public AnimatedDownloadTask(Illust illust,
            DirectoryInfo storageRoot,
            IDbContextFactory<EHunterDbContext> eFactory,
            IDbContextFactory<PixivDbContext> pFactory)
            : base(illust, storageRoot, eFactory, pFactory)
        {
            if (!illust.IsAnimated)
                throw new InvalidOperationException("Please use non-animated download task.");
        }

        protected override async IAsyncEnumerable<(double progress, ImageEntry? entry)> DownloadAndReturnMetadataAsync(string directoryPart)
        {
            string filename = $"{Illust.Id}.gif";
            var details = await Illust.GetAnimatedDetailAsync().ConfigureAwait(false);

            string relativeFilename = Path.Combine(directoryPart, filename);
            using var fs = File.Create(Path.Combine(StorageRoot.FullName, relativeFilename), 8192, FileOptions.Asynchronous);

            using var response = await details.GetZipAsync().ConfigureAwait(false);
            using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            long? length = response.Content.Headers.ContentLength;

            using var mms = new MemoryStream();
            byte[] buffer = new byte[8192];
            int bytesRead;
            long totalBytesRead = 0;

            while ((bytesRead = await responseStream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
            {
                await mms.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
                totalBytesRead += bytesRead;

                double pageProgress = (double)totalBytesRead / length ?? 0;

                yield return (pageProgress, null);
            }

            mms.Seek(0, SeekOrigin.Begin);
            await GifHelper.ComposeGifAsync(new ZipArchive(mms),
                details.Frames.Select(x => (x.File, x.Delay)),
                fs)
                .ConfigureAwait(false);

            await fs.FlushAsync().ConfigureAwait(false);

            yield return (1, new(ImageType.Animated, relativeFilename)
            {
                PostOrderId = 0
            });
        }
    }
}
