using System;
using System.Collections.Generic;
using System.IO;
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

        protected override async IAsyncEnumerable<ImageEntry> DownloadAndReturnMetadataAsync(string directoryPart)
        {
            string filename = $"{Illust.Id}.gif";
            var details = await Illust.GetAnimatedDetailAsync().ConfigureAwait(false);
            int current = 0;

            string relativeFilename = Path.Combine(directoryPart, filename);
            using var fs = File.Create(Path.Combine(StorageRoot.FullName, relativeFilename), 8192, FileOptions.Asynchronous);

            await GifHelper.ComposeGifAsync(details, fs,
                () => ProgressObservable.Next(++current / (double)details.Frames.Length))
                .ConfigureAwait(false);

            await fs.FlushAsync().ConfigureAwait(false);

            yield return new(ImageType.Animated, relativeFilename)
            {
                PostOrderId = 0
            };
        }
    }
}
