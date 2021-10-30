using System;
using System.Buffers;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Data;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Data;
using EHunter.Pixiv.Media;
using EHunter.Settings;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Pixiv.Services.Download
{
    [Export]
    public class DownloaderService
    {
        private readonly PixivClient _client;
        private readonly IDbContextFactoryResolver<EHunterDbContext> _eHunterContextResolver;
        private readonly IDbContextFactoryResolver<PixivDbContext> _pixivDbContextResolver;
        private readonly IStorageSetting _storageSetting;

        [ImportingConstructor]
        public DownloaderService(
            ICustomResolver<PixivClient> pixivClientResolver,
            IDbContextFactoryResolver<EHunterDbContext> eHunterContextResolver,
            IDbContextFactoryResolver<PixivDbContext> pixivDbContextResolver,
            IStorageSetting storageSetting)
        {
            _client = pixivClientResolver.Resolve();
            _eHunterContextResolver = eHunterContextResolver;
            _pixivDbContextResolver = pixivDbContextResolver;
            _storageSetting = storageSetting;
        }

        public async IAsyncEnumerable<int> GetResumableDownloads()
        {
            var pFactory = _pixivDbContextResolver.Resolve();
            if (pFactory is null)
                yield break;

            using var pContext = pFactory.CreateDbContext();

            var groups = (await pContext.PixivPendingDownloads.AsQueryable()
                .OrderBy(x => x.Time)
                .ToArrayAsync()
                .ConfigureAwait(false))
                .GroupBy(x => x.PId);

            foreach (var g in groups)
            {
                try
                {
                    using var process = Process.GetProcessById(g.Key);
                    if (!process.HasExited)
                        continue;
                }
                catch
                {
                    // No such process running.
                }

                foreach (var p in g)
                {
                    p.PId = Environment.ProcessId;
                    try
                    {
                        await pContext.SaveChangesAsync().ConfigureAwait(false);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        continue;
                    }

                    yield return p.ArtworkId;
                }
            }
        }

        public ValueTask<DownloadableState> CanDownloadAsync(int artworkId)
        {
            var pFactory = _pixivDbContextResolver.Resolve();
            var eFactory = _eHunterContextResolver.Resolve();
            if (pFactory is null || eFactory is null)
                return new(DownloadableState.ServiceUnavailable);

            string? storageRoot = _storageSetting.StorageRoot.Value;

            if (storageRoot is null)
                return new(DownloadableState.ServiceUnavailable);

            return new(Task.Run(async () =>
            {
                if (!Directory.Exists(storageRoot))
                    return DownloadableState.ServiceUnavailable;
                try
                {
                    using var pContext = pFactory.CreateDbContext();
                    if (await pContext.PixivPendingDownloads
                        .AsQueryable()
                        .AnyAsync(x => x.ArtworkId == artworkId)
                        .ConfigureAwait(false))
                        return DownloadableState.AlreadyPending;
                }
                catch
                {
                    return DownloadableState.ServiceUnavailable;
                }

                try
                {
                    using var eContext = eFactory.CreateDbContext();
                    if (await eContext.Posts
                        .AsQueryable()
                        .AnyAsync(x => x.Provider == "Pixiv:Illust" && x.Identifier == artworkId)
                        .ConfigureAwait(false))
                        return DownloadableState.AlreadyDownloaded;
                }
                catch
                {
                    return DownloadableState.ServiceUnavailable;
                }

                return DownloadableState.CanDownload;
            }));
        }

        public async Task AddToPendingAsync(int illustId)
        {
            var pFactory = _pixivDbContextResolver.Resolve()
                ?? throw new InvalidOperationException("No database connetion");

            using var pContext = pFactory.CreateDbContext();
            pContext.PixivPendingDownloads.Add(new PendingDownload
            {
                ArtworkId = illustId,
                Time = DateTimeOffset.Now,
                PId = Environment.ProcessId
            });
            await pContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<bool> RemoveFromPendingAsync(int illustId)
        {
            var pFactory = _pixivDbContextResolver.Resolve()
                ?? throw new InvalidOperationException("No database connetion");

            using var pContext = pFactory.CreateDbContext();
            var pending = pContext.PixivPendingDownloads.Find(illustId);
            bool removed = pending?.PId == Environment.ProcessId;

            if (removed)
                pContext.PixivPendingDownloads.Remove(pending!);
            await pContext.SaveChangesAsync().ConfigureAwait(false);

            return removed;
        }

        public async Task DownloadByIdAsync(int artworkId,
            DateTimeOffset? favoratedTime = null,
            Action<double>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            var illust = await _client.GetIllustDetailAsync(artworkId, cancellationToken).ConfigureAwait(false);
            await DownloadAsync(illust, favoratedTime, onProgress, cancellationToken).ConfigureAwait(false);
        }

        public async Task DownloadAsync(Illust illust,
            DateTimeOffset? favoratedTime = null,
            Action<double>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            var eFactory = _eHunterContextResolver.Resolve()
                ?? throw new InvalidOperationException("No database connetion");
            string? storageRoot = _storageSetting.StorageRoot.Value
                ?? throw new InvalidOperationException("No storage");

            using var eContext = eFactory.CreateDbContext();

            if (await eContext.Posts.AsQueryable()
                .AnyAsync(x => x.Provider == "Pixiv:Illust" && x.Identifier == illust.Id, cancellationToken)
                .ConfigureAwait(false))
            {
                // already downloaded
                return;
            }

            var post = new Post
            {
                PublishedTime = illust.Created,
                FavoritedTime = favoratedTime ?? DateTimeOffset.Now,
                Title = illust.Title,
                DetailText = illust.Description,
                Url = new Uri($"https://www.pixiv.net/artworks/{illust.Id}"),
                Provider = "Pixiv:Illust",
                Identifier = illust.Id
            };

            (string Relative, string Absolute) WithDirectory(string filename)
            {
                string relative = Path.Combine("Pixiv", illust.User.Id.ToString(NumberFormatInfo.InvariantInfo), filename);
                string absolute = Path.Combine(storageRoot, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(absolute)!);
                return (relative, absolute);
            }

            if (illust.IsAnimated)
            {
                string filename = $"{illust.Id}.gif";
                var details = await illust.GetAnimatedDetailAsync(cancellationToken).ConfigureAwait(false);

                var (relative, absolute) = WithDirectory(filename);
                using var fs = File.Create(absolute, 8192, FileOptions.Asynchronous);

                using var mms = new MemoryStream();

                using (var response = await details.GetZipAsync(cancellationToken).ConfigureAwait(false))
                    await CopyWithProgressAsync(response, mms, onProgress, cancellationToken).ConfigureAwait(false);

                mms.Seek(0, SeekOrigin.Begin);

                using (var zipArchive = new ZipArchive(mms))
                    await GifHelper.ComposeGifAsync(zipArchive,
                        details.Frames.Select(x => (x.File, x.Delay)),
                        fs,
                        cancellationToken)
                        .ConfigureAwait(false);

                await fs.FlushAsync(cancellationToken).ConfigureAwait(false);

                post.Images.Add(new(ImageType.Animated, relative)
                {
                    PostOrderId = 0
                });
            }
            else
            {
                for (int p = 0; p < illust.Pages.Count; p++)
                {
                    var page = illust.Pages[p];
                    using var response = await page.Original.RequestAsync(cancellationToken).ConfigureAwait(false);

                    string filename = response.Content.Headers.ContentDisposition?.FileName
                        ?? $"{illust.Id}_p{page.Index}.jpg";
                    var (relative, absolute) = WithDirectory(filename);

                    using var fs = File.Create(absolute, 8192, FileOptions.Asynchronous);

                    await CopyWithProgressAsync(response, fs, pageProgress => onProgress?.Invoke((pageProgress + p) / illust.Pages.Count), cancellationToken)
                        .ConfigureAwait(false);

                    await fs.FlushAsync(cancellationToken).ConfigureAwait(false);

                    post.Images.Add(new(ImageType.Static, relative)
                    {
                        PostOrderId = p
                    });
                    onProgress?.Invoke((p + 1) / (double)illust.Pages.Count);
                }
            }

            var tags = await illust.Tags.Select(x => (tagScopeName: "Pixiv:Tag", tagName: x.Name))
                .Append((tagScopeName: "Pixiv:ArtistId", tagName: illust.User.Id.ToString(NumberFormatInfo.InvariantInfo))).ToAsyncEnumerable()
                .SelectMany(x => eContext.MapTag(x.tagScopeName, x.tagName))
                .Distinct()
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            eContext.Posts.Add(post);
            if (illust.Pages.Count == 1)
            {
                post.Images[0].Tags.AddRange(tags.Select(x => new ImageTag(x.tagScopeName, x.tagName)));
            }
            else
            {
                var gallery = new PostGallery { Name = illust.Title, Post = post };
                gallery.Tags.AddRange(tags.Select(x => new GalleryTag(x.tagScopeName, x.tagName)));
                eContext.Add(gallery);
            }

            await eContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        protected static async Task CopyWithProgressAsync(
            HttpResponseMessage response,
            Stream destination,
            Action<double>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            long? length = response.Content.Headers.ContentLength;

            using var memoryOwner = MemoryPool<byte>.Shared.Rent(8192);
            var buffer = memoryOwner.Memory;
            int bytesRead;
            long totalBytesRead = 0;

            while ((bytesRead = await responseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
            {
                await destination.WriteAsync(buffer[..bytesRead], cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;

                onProgress?.Invoke((double)totalBytesRead / length ?? 0);
            }
        }
    }

    public enum DownloadableState
    {
        ServiceUnavailable,
        AlreadyPending,
        AlreadyDownloaded,
        CanDownload
    }
}
