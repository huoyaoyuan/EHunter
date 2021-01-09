using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Models;
using EHunter.Pixiv.Services.Download;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Download
{
    public sealed class DownloadManager : ObservableObject, IDisposable
    {
        private readonly PixivSetting _setting;
        internal readonly DownloaderService Downloader;
        private readonly IDisposable _settingSubscriber;

        public DownloadManager(PixivSetting setting, DownloaderService downloader, ICustomResolver<PixivClient> clientResolver)
        {
            _setting = setting;
            Downloader = downloader;

            _settingSubscriber = _setting.MaxDownloadsInParallelChanged.Subscribe(
                _ => CheckStartNew());
            ResumeDownloads();

            async void ResumeDownloads()
            {
                var client = clientResolver.Resolve();
                try
                {
                    await foreach (var task in downloader.GetResumableDownloads().ConfigureAwait(true))
                        CreateAndAddVM(task);
                }
                catch
                {
                    return;
                }
            }
        }

        #region Task2
        private readonly Dictionary<int, WeakReference<DownloadableVM2>> _wrById = new();
        private void PruneDictionary()
        {
            foreach (int id in _wrById.Keys.ToArray())
            {
                if (!_wrById[id].TryGetTarget(out _))
                    _wrById.Remove(id);
            }
        }
        private int _pruneCounter;
        public DownloadableVM2 GetOrAddDownloadable(Illust illust)
        {
            if (_wrById.TryGetValue(illust.Id, out var wr))
            {
                if (wr.TryGetTarget(out var vm))
                {
                    return vm;
                }
                else
                {
                    vm = new DownloadableVM2(illust, this);
                    wr.SetTarget(vm);
                    return vm;
                }
            }
            else
            {
                if (++_pruneCounter % 100 == 0)
                {
                    _pruneCounter = 0;
                    PruneDictionary();
                }

                var vm = new DownloadableVM2(illust, this);
                _wrById.Add(illust.Id, new(vm));
                return vm;
            }
        }
        #endregion

        public ObservableCollection<DownloadTaskVM> DownloadTasks { get; } = new();
        private readonly Dictionary<int, DownloadTaskVM> _taskById = new();

        private int _pendingDownloads;
        public int PendingDownloads
        {
            get => _pendingDownloads;
            private set => SetProperty(ref _pendingDownloads, value);
        }

        private int _activeDownloads;
        public int ActiveDownloads
        {
            get => _activeDownloads;
            private set => SetProperty(ref _activeDownloads, value);
        }

        public DownloadTaskVM? GetExistingTask(int illustId)
            => _taskById.GetValueOrDefault(illustId);

        public DownloadableIllustVM GetDownloadableVM(Illust illust)
        {
            var existingDownload = GetExistingTask(illust.Id);
            return new(this, illust, existingDownload,
                existingDownload is null
                ? Downloader.CanDownloadAsync(illust.Id)
                : new(DownloadableState.AlreadyPending));
        }

        internal void QueueOne2(DownloadableVM2 vm)
        {
            QueuedTasks2.Add(vm);
            CheckStartNew2();
        }

        internal void CompleteOne2(DownloadableVM2 _)
        {
            PendingDownloads--;
            ActiveDownloads--;
            CheckStartNew();
        }

        public ObservableCollection<DownloadableVM2> QueuedTasks2 { get; } = new();

        private void CheckStartNew2()
        {
            while (ActiveDownloads < _setting.MaxDownloadsInParallel)
            {
                var task = QueuedTasks2.FirstOrDefault(x => x.State == DownloadableState2.Waiting);
                if (task is null)
                    return;

                ActiveDownloads++;
                task.Start();
            }
        }

        internal async Task<DownloadTaskVM> CreateDownloadTaskAsync(Illust illust)
        {
            var task = await Downloader.CreateDownloadTaskAsync(illust).ConfigureAwait(true);
            return CreateAndAddVM(task);
        }

        private DownloadTaskVM CreateAndAddVM(DownloadTask task)
        {
            var vm = new DownloadTaskVM(this, task);
            _taskById.Add(task.Illust.Id, vm);
            DownloadTasks.Add(vm);

            PendingDownloads++;
            CheckStartNew();

            return vm;
        }

        internal void CompleteOne(DownloadTaskVM _)
        {
            PendingDownloads--;
            ActiveDownloads--;
            CheckStartNew();
        }

        private void CheckStartNew()
        {
            while (ActiveDownloads < _setting.MaxDownloadsInParallel)
            {
                var task = DownloadTasks.FirstOrDefault(x => x.State == DownloadTaskState.Waiting);
                if (task is null)
                    return;

                ActiveDownloads++;
                task.Start();
            }
        }

        public void Prune()
        {
            for (int i = 0; i < DownloadTasks.Count; i++)
                if (DownloadTasks[i] is
                    {
                        State: not (DownloadTaskState.Waiting or DownloadTaskState.Active),
                        Illust: { Id: int id }
                    })
                {
                    DownloadTasks.RemoveAt(i--);
                    _taskById.Remove(id);
                }
        }

        public void Dispose() => _settingSubscriber.Dispose();
    }
}
