using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Data;
using EHunter.Data.Pixiv;
using Meowtrix.PixivApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Pixiv.Services.Download
{
    public class NonAnimatedDownloadTask : DownloadTask
    {
        public NonAnimatedDownloadTask(Illust illust,
            DirectoryInfo storageRoot,
            IDbContextFactory<EHunterDbContext> eFactory,
            IDbContextFactory<PixivDbContext> pFactory)
            : base(illust, storageRoot, eFactory, pFactory)
        {
            if (illust.IsAnimated)
                throw new InvalidOperationException("Please use animated download task.");
        }

        protected override async Task DownloadAsync(
            IList<ImageEntry> entries,
            Action<double>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            for (int p = 0; p < Illust.Pages.Count; p++)
            {
                var page = Illust.Pages[p];
                using var response = await page.Original.RequestAsync(cancellationToken).ConfigureAwait(false);

                string filename = response.Content.Headers.ContentDisposition?.FileName
                    ?? $"{Illust.Id}_p{page.Index}.jpg";
                var (relative, absolute) = WithDirectory(filename);

                using var fs = File.Create(absolute, 8192, FileOptions.Asynchronous);

                await ReadAsync(response, fs, pageProgress => onProgress?.Invoke((pageProgress + p) / Illust.Pages.Count), cancellationToken)
                    .ConfigureAwait(false);

                await fs.FlushAsync(cancellationToken).ConfigureAwait(false);

                entries.Add(new(ImageType.Static, relative)
                {
                    PostOrderId = p
                });
                onProgress?.Invoke((p + 1) / (double)Illust.Pages.Count);
            }
        }
    }
}
