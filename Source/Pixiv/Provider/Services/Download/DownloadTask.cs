using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Data;
using EHunter.Pixiv.Data;
using Meowtrix.PixivApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Pixiv.Services.Download
{
    public abstract class DownloadTask
    {
        public Illust Illust { get; }

        private protected readonly DirectoryInfo StorageRoot;
        private readonly IDbContextFactory<EHunterDbContext> _eFactory;
        private readonly IDbContextFactory<PixivDbContext> _pFactory;

        private protected DownloadTask(Illust illust,
            DirectoryInfo storageRoot,
            IDbContextFactory<EHunterDbContext> eFactory,
            IDbContextFactory<PixivDbContext> pFactory)
        {
            Illust = illust;
            StorageRoot = storageRoot;
            _eFactory = eFactory;
            _pFactory = pFactory;
        }

        public async Task RunAsync(DateTimeOffset? favoratedTime = null,
            Action<double>? onProgress = null,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            IEnumerable<(string tagScopeName, string tagName)> tagsInfo =
                Illust.Tags.Select(x => ("Pixiv:Tag", x.Name))
                .Append(("Pixiv:ArtistId", Illust.User.Id.ToString(NumberFormatInfo.InvariantInfo)));

            var post = new Post
            {
                PublishedTime = Illust.Created,
                FavoritedTime = favoratedTime ?? DateTimeOffset.Now,
                Title = Illust.Title,
                DetailText = Illust.Description,
                Url = new Uri($"https://www.pixiv.net/artworks/{Illust.Id}"),
                Provider = "Pixiv:Illust",
                Identifier = Illust.Id
            };

            bool shouldRemovePending = true;

            try
            {
                await DownloadAsync(post.Images, onProgress, cancellationToken).ConfigureAwait(false);

                using var eContext = _eFactory.CreateDbContext();

                var tags = await tagsInfo
                    .ToAsyncEnumerable()
                    .SelectMany(x => eContext.MapTag(x.tagScopeName, x.tagName))
                    .Distinct()
                    .ToArrayAsync(cancellationToken)
                    .ConfigureAwait(false);

                eContext.Posts.Add(post);
                if (Illust.Pages.Count == 1)
                {
                    post.Images[0].Tags.AddRange(tags.Select(x => new ImageTag(x.tagScopeName, x.tagName)));
                }
                else
                {
                    var gallery = new PostGallery { Name = Illust.Title, Post = post };
                    gallery.Tags.AddRange(tags.Select(x => new GalleryTag(x.tagScopeName, x.tagName)));
                    eContext.Add(gallery);
                }

                await eContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                // should remove pending
                throw;
            }
            catch
            {
                shouldRemovePending = false;
                throw;
            }
            finally
            {
                if (shouldRemovePending)
                {
                    using var pContext = _pFactory.CreateDbContext();
                    var pendingTask = pContext.PixivPendingDownloads.Find(Illust.Id);
                    pContext.PixivPendingDownloads.Remove(pendingTask);
                    await pContext.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        protected abstract Task DownloadAsync(
            IList<ImageEntry> entries,
            Action<double>? onProgress = null,
            CancellationToken cancellationToken = default);

        protected static async Task ReadAsync(
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

        protected (string Relative, string Absolute) WithDirectory(string filename)
        {
            string relative = Path.Combine("Pixiv", Illust.User.Id.ToString(NumberFormatInfo.InvariantInfo), filename);
            string absolute = Path.Combine(StorageRoot.FullName, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(absolute)!);
            return (relative, absolute);
        }
    }

    internal static class EnumerableExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> value)
        {
            if (list is List<T> l)
            {
                l.AddRange(value);
            }
            else
            {
                foreach (var v in value)
                    list.Add(v);
            }
        }
    }
}
