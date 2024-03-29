﻿using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Settings;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Services.Download
{
    public sealed partial class DownloadManager : ObservableObject, IDisposable
    {
        private readonly PixivSetting _setting;
        internal readonly DownloaderService Downloader;
        private readonly IDisposable _settingSubscriber;

        [ObservableProperty]
        private int _pendingDownloads;

        [ObservableProperty]
        private int _activeDownloads;

        public DownloadManager(PixivSetting setting, DownloaderService downloader, ICustomResolver<PixivClient> clientResolver)
        {
            _setting = setting;
            Downloader = downloader;
            _settingSubscriber = _setting.MaxDownloadsInParallel.Subscribe(
                _ => CheckStartNew());
            ResumeDownloads();

            async void ResumeDownloads()
            {
                var client = clientResolver.Resolve();
                try
                {
                    await foreach (int id in downloader.GetResumableDownloads().ConfigureAwait(true))
                    {
                        var illust = await client.GetIllustDetailAsync(id).ConfigureAwait(true);
                        var vm = GetOrAddDownloadable(illust);
                        vm.SetWaiting();
                        QueueOne(vm);
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        private readonly Dictionary<int, WeakReference<DownloadTask>> _wrById = new();
        private void PruneDictionary()
        {
            foreach (int id in _wrById.Keys.ToArray())
            {
                if (!_wrById[id].TryGetTarget(out _))
                    _wrById.Remove(id);
            }
        }
        private int _pruneCounter;
        public DownloadTask GetOrAddDownloadable(Illust illust)
        {
            if (_wrById.TryGetValue(illust.Id, out var wr))
            {
                if (wr.TryGetTarget(out var vm))
                {
                    return vm;
                }
                else
                {
                    vm = new DownloadTask(illust, this);
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

                var vm = new DownloadTask(illust, this);
                _wrById.Add(illust.Id, new(vm));
                return vm;
            }
        }

        internal void QueueOne(DownloadTask vm)
        {
            if (!_queuedTasks.Items.Contains(vm))
                _queuedTasks.Add(vm);
            CheckStartNew();
        }

        internal void CompleteOne(DownloadTask _)
        {
            PendingDownloads--;
            ActiveDownloads--;
            CheckStartNew();
        }

        private readonly SourceList<DownloadTask> _queuedTasks = new();
        public IObservableList<DownloadTask> QueuedTasks => _queuedTasks;

        private void CheckStartNew()
        {
            while (ActiveDownloads < _setting.MaxDownloadsInParallel.Value)
            {
                var task = _queuedTasks.Items.FirstOrDefault(x => x.State == IllustDownloadState.Waiting);
                if (task is null)
                    return;

                ActiveDownloads++;
                task.Start();
            }
        }

        public void Prune()
        {
            _queuedTasks.RemoveMany(_queuedTasks.Items
                .Where(x => x.State is not (IllustDownloadState.Waiting or IllustDownloadState.Active)));
        }

        public void Dispose()
        {
            _settingSubscriber.Dispose();
            _queuedTasks.Dispose();
        }
    }
}
