using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using EHunter.Data;
using EHunter.Data.Pixiv;
using Meowtrix.PixivApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Provider.Pixiv.Services.Download
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

        public async void Start(DateTimeOffset? favoratedTime = null)
        {
            await Task.Yield();

            try
            {
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

                string directoryPart = Path.Combine("Pixiv", Illust.User.Id.ToString(NumberFormatInfo.InvariantInfo));
                string directory = Path.Combine(StorageRoot.FullName, directoryPart);
                Directory.CreateDirectory(directory);

#pragma warning disable CA1508 // false positive
                await foreach (var entry in DownloadAndReturnMetadataAsync(directoryPart).ConfigureAwait(false))
#pragma warning restore CA1508
                    post.Images.Add(entry);

                using var eContext = _eFactory.CreateDbContext();
                using var pContext = _pFactory.CreateDbContext();

                // Has issue with DbContext factory
                // using var transaction = pContext.UseTransactionWith(eContext);

                var tags = await tagsInfo
                    .ToAsyncEnumerable()
                    .SelectMany(x => eContext.MapTag(x.tagScopeName, x.tagName))
                    .Distinct()
                    .ToArrayAsync()
                    .ConfigureAwait(false);

                var pendingTask = pContext.PixivPendingDownloads.Find(Illust.Id);
                pContext.PixivPendingDownloads.Remove(pendingTask);
                await pContext.SaveChangesAsync().ConfigureAwait(false);

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

                await eContext.SaveChangesAsync().ConfigureAwait(false);

                // await transaction.CommitAsync().ConfigureAwait(false);

                ProgressObservable.Complete();
            }
            catch (Exception e)
            {
                ProgressObservable.Error(e);
            }
        }

        protected abstract IAsyncEnumerable<ImageEntry> DownloadAndReturnMetadataAsync(string directoryPart);

        private protected readonly ProgressObservable<double> ProgressObservable = new();
        public IObservable<double> Progress => ProgressObservable;
    }

    internal class ProgressObservable<T> : IObservable<T>
    {
        private readonly List<IObserver<T>> _observers = new();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_observers)
                _observers.Add(observer);
            return new UnObservable(this, observer);
        }

        private class UnObservable : IDisposable
        {
            private readonly ProgressObservable<T> _owner;
            private readonly IObserver<T> _observer;

            public UnObservable(ProgressObservable<T> owner, IObserver<T> observer)
            {
                _owner = owner;
                _observer = observer;
            }

            public void Dispose()
            {
                lock (_owner._observers)
                    _owner._observers.Remove(_observer);
            }
        }

        public void Next(T value)
        {
            lock (_observers)
                foreach (var o in _observers)
                    o.OnNext(value);
        }

        public void Error(Exception e)
        {
            lock (_observers)
                foreach (var o in _observers)
                    o.OnError(e);
        }

        public void Complete()
        {
            lock (_observers)
                foreach (var o in _observers)
                    o.OnCompleted();
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
