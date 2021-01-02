using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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

        protected override async IAsyncEnumerable<(double progress, ImageEntry? entry)> DownloadAndReturnMetadataAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            string filename = $"{Illust.Id}.gif";
            var details = await Illust.GetAnimatedDetailAsync().ConfigureAwait(false);

            var (relative, absolute) = WithDirectory(filename);
            using var fs = File.Create(absolute, 8192, FileOptions.Asynchronous);

            using var response = await details.GetZipAsync(cancellationToken).ConfigureAwait(false);
            using var mms = new MemoryStream();
            await foreach (double p in ReadWithProgress(response, mms, cancellationToken))
                yield return (p, null);

            mms.Seek(0, SeekOrigin.Begin);
            await GifHelper.ComposeGifAsync(new ZipArchive(mms),
                details.Frames.Select(x => (x.File, x.Delay)),
                fs,
                cancellationToken)
                .ConfigureAwait(false);

            await fs.FlushAsync(cancellationToken).ConfigureAwait(false);

            yield return (1, new(ImageType.Animated, relative)
            {
                PostOrderId = 0
            });
        }
    }
}
