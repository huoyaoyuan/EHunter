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
    public class DownloadTask
    {
        public Illust Illust { get; }

        private readonly DirectoryInfo _storageRoot;
        private readonly IDbContextFactory<EHunterDbContext> _eFactory;
        private readonly IDbContextFactory<PixivDbContext> _pFactory;

        public DownloadTask(Illust illust,
            DirectoryInfo storageRoot,
            IDbContextFactory<EHunterDbContext> eFactory,
            IDbContextFactory<PixivDbContext> pFactory)
        {
            Illust = illust;
            _storageRoot = storageRoot;
            _eFactory = eFactory;
            _pFactory = pFactory;
        }

        public async void Start()
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
                    FavoritedTime = DateTimeOffset.Now,
                    Title = Illust.Title,
                    DetailText = Illust.Description,
                    Url = new Uri($"https://www.pixiv.net/artworks/{Illust.Id}")
                };

                string directoryPart = Path.Combine("Pixiv", Illust.User.Id.ToString(NumberFormatInfo.InvariantInfo));
                string directory = Path.Combine(_storageRoot.FullName, directoryPart);
                Directory.CreateDirectory(directory);

                if (Illust.IsAnimated)
                {
                    string filename = Path.Combine(directory, $"{Illust.Id}.gif");
                    // TODO: save gif
                }
                else
                {
                    for (int p = 0; p < Illust.Pages.Count; p++)
                    {
                        var page = Illust.Pages[p];
                        using var response = await page.Original.RequestAsync().ConfigureAwait(false);

                        long? length = response.Content.Headers.ContentLength;
                        string filename = response.Content.Headers.ContentDisposition?.FileName
                            ?? $"{Illust.Id}_p{page.Index}.jpg";
                        string relativeFilename = Path.Combine(directoryPart, filename);

                        using var fs = File.Create(Path.Combine(_storageRoot.FullName, relativeFilename), 8192, FileOptions.Asynchronous);
                        byte[] buffer = new byte[8192];
                        long totalBytesRead = 0;

                        using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                        int bytesRead;
                        while ((bytesRead = await responseStream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
                        {
                            await fs.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
                            totalBytesRead += bytesRead;

                            double pageProgress = (double)totalBytesRead / length ?? 0;

                            _progressObservable.Next((pageProgress + p) / Illust.Pages.Count);
                        }

                        await fs.FlushAsync().ConfigureAwait(false);
                        _progressObservable.Next((p + 1) / (double)Illust.Pages.Count);

                        post.Images.Add(new(ImageType.Static, relativeFilename)
                        {
                            PostOrderId = p
                        });
                    }
                }

                using var eContext = _eFactory.CreateDbContext();
                using var pContext = _pFactory.CreateDbContext();
                using var transaction = pContext.UseTransactionWith(eContext);

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

                await transaction.CommitAsync().ConfigureAwait(false);

                _progressObservable.Complete();
            }
            catch (Exception e)
            {
                _progressObservable.Error(e);
            }
        }

        private readonly ProgressObservable<double> _progressObservable = new();
        public IObservable<double> Progress => _progressObservable;
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
