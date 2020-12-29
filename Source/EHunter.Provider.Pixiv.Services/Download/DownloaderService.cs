using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using EHunter.Data;
using EHunter.Data.Pixiv;
using EHunter.DependencyInjection;
using EHunter.Settings;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Provider.Pixiv.Services.Download
{
    public class DownloaderService
    {
        private readonly PixivClient _client;
        private readonly ICustomResolver<IDbContextFactory<EHunterDbContext>?> _eHunterContextResolver;
        private readonly ICustomResolver<IDbContextFactory<PixivDbContext>?> _pixivDbContextResolver;
        private readonly IStorageSetting _storageSetting;

        public DownloaderService(
            ICustomResolver<PixivClient> pixivClientResolver,
            ICustomResolver<IDbContextFactory<EHunterDbContext>?> eHunterContextResolver,
            ICustomResolver<IDbContextFactory<PixivDbContext>?> pixivDbContextResolver,
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

            var groups = pContext.PixivPendingDownloads.AsQueryable()
                .OrderBy(x => x.Time)
                .GroupBy(x => x.PId)
                .AsAsyncEnumerable();

#pragma warning disable CA1508 // false positive
            await foreach (var g in groups.ConfigureAwait(false))
#pragma warning restore CA1508
            {
                try
                {
                    using var process = Process.GetProcessById(g.Key);
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

        public async ValueTask<bool?> CanDownloadAsync(int artworkId)
        {
            var pFactory = _pixivDbContextResolver.Resolve();
            var eFactory = _eHunterContextResolver.Resolve();
            if (pFactory is null || eFactory is null)
                return null;

            if (_storageSetting.StorageRoot?.Exists != true)
                return null;

            try
            {
                using var pContext = pFactory.CreateDbContext();
                if (await pContext.PixivPendingDownloads
                    .AsQueryable()
                    .AnyAsync(x => x.ArtworkId == artworkId)
                    .ConfigureAwait(false))
                    return false;
            }
            catch
            {
                return null;
            }

            try
            {
                using var eContext = eFactory.CreateDbContext();
                if (await eContext.Posts
                    .AsQueryable()
                    .AnyAsync(x => x.Provider == "Pixiv:Illust" && x.Identifier == artworkId)
                    .ConfigureAwait(false))
                    return false;
            }
            catch
            {
                return null;
            }

            return true;
        }

        public async Task<DownloadTask> CreateDownloadTaskAsync(int artworkId)
            => await CreateDownloadTaskAsync(
                await _client.GetIllustDetailAsync(artworkId)
                .ConfigureAwait(false))
            .ConfigureAwait(false);

        public async Task<DownloadTask> CreateDownloadTaskAsync(Illust illust)
        {
            var pFactory = _pixivDbContextResolver.Resolve()
                ?? throw new InvalidOperationException("No database connetion");
            var eFactory = _eHunterContextResolver.Resolve()
                ?? throw new InvalidOperationException("No database connetion");
            var storageRoot = _storageSetting.StorageRoot
                ?? throw new InvalidOperationException("No storage");

            using var pContext = pFactory.CreateDbContext();
            pContext.PixivPendingDownloads.Add(new PendingDownload
            {
                ArtworkId = illust.Id,
                Time = DateTimeOffset.Now,
                PId = Environment.ProcessId
            });
            await pContext.SaveChangesAsync().ConfigureAwait(false);
            return illust.IsAnimated
                ? throw new NotImplementedException()
                : new NonAnimatedDownloadTask(illust, storageRoot, eFactory, pFactory);
        }
    }
}
