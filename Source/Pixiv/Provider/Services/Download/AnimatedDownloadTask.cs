using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Data;
using EHunter.Pixiv.Data;
using Meowtrix.PixivApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Pixiv.Services.Download
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

        protected override async Task DownloadAsync(
            IList<ImageEntry> entries,
            Action<double>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            string filename = $"{Illust.Id}.gif";
            var details = await Illust.GetAnimatedDetailAsync(cancellationToken).ConfigureAwait(false);

            var (relative, absolute) = WithDirectory(filename);
            using var fs = File.Create(absolute, 8192, FileOptions.Asynchronous);

            using var mms = new MemoryStream();

            using (var response = await details.GetZipAsync(cancellationToken).ConfigureAwait(false))
                await ReadAsync(response, mms, onProgress, cancellationToken).ConfigureAwait(false);

            mms.Seek(0, SeekOrigin.Begin);

            using (var zipArchive = new ZipArchive(mms))
                await GifHelper.ComposeGifAsync(zipArchive,
                    details.Frames.Select(x => (x.File, x.Delay)),
                    fs,
                    cancellationToken)
                    .ConfigureAwait(false);

            await fs.FlushAsync(cancellationToken).ConfigureAwait(false);

            entries.Add(new(ImageType.Animated, relative)
            {
                PostOrderId = 0
            });
        }
    }
}
