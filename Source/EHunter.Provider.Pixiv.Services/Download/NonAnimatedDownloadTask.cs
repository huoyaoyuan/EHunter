using System;
using System.Collections.Generic;
using System.IO;
using EHunter.Data;
using EHunter.Data.Pixiv;
using Meowtrix.PixivApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Provider.Pixiv.Services.Download
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

        protected override async IAsyncEnumerable<(double progress, ImageEntry? entry)> DownloadAndReturnMetadataAsync(string directoryPart)
        {
            for (int p = 0; p < Illust.Pages.Count; p++)
            {
                var page = Illust.Pages[p];
                using var response = await page.Original.RequestAsync().ConfigureAwait(false);

                long? length = response.Content.Headers.ContentLength;
                string filename = response.Content.Headers.ContentDisposition?.FileName
                    ?? $"{Illust.Id}_p{page.Index}.jpg";
                string relativeFilename = Path.Combine(directoryPart, filename);

                using var fs = File.Create(Path.Combine(StorageRoot.FullName, relativeFilename), 8192, FileOptions.Asynchronous);
                byte[] buffer = new byte[8192];
                long totalBytesRead = 0;

                using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                int bytesRead;
                while ((bytesRead = await responseStream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
                {
                    await fs.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
                    totalBytesRead += bytesRead;

                    double pageProgress = (double)totalBytesRead / length ?? 0;

                    yield return ((pageProgress + p) / Illust.Pages.Count, null);
                }

                await fs.FlushAsync().ConfigureAwait(false);

                yield return ((p + 1) / (double)Illust.Pages.Count, new(ImageType.Static, relativeFilename)
                {
                    PostOrderId = p
                });
            }
        }
    }
}
