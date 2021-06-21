using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Services.Download;
using EHunter.Pixiv.Services.Images;
using EHunter.Pixiv.Settings;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Download
{
    [Export, Shared]
    [ObservableProperty("PendingDownloads", typeof(int), IsSetterPublic = false)]
    [ObservableProperty("ActiveDownloads", typeof(int), IsSetterPublic = false)]
    public sealed partial class DownloadManager : ObservableObject, IDisposable
    {
        private readonly PixivSetting _setting;
        internal readonly DownloaderService Downloader;
        private readonly PixivImageService _imageService;
        private readonly IDisposable _settingSubscriber;

        [ImportingConstructor]
        public DownloadManager(PixivSetting setting, DownloaderService downloader, PixivImageService imageService, ICustomResolver<PixivClient> clientResolver)
        {
            _setting = setting;
            Downloader = downloader;
            _imageService = imageService;
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

        private readonly Dictionary<int, WeakReference<IllustDownloadVM>> _wrById = new();
        private void PruneDictionary()
        {
            foreach (int id in _wrById.Keys.ToArray())
            {
                if (!_wrById[id].TryGetTarget(out _))
                    _wrById.Remove(id);
            }
        }
        private int _pruneCounter;
        public IllustDownloadVM GetOrAddDownloadable(Illust illust)
        {
            if (_wrById.TryGetValue(illust.Id, out var wr))
            {
                if (wr.TryGetTarget(out var vm))
                {
                    return vm;
                }
                else
                {
                    vm = new IllustDownloadVM(illust, this, _imageService);
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

                var vm = new IllustDownloadVM(illust, this, _imageService);
                _wrById.Add(illust.Id, new(vm));
                return vm;
            }
        }

        internal void QueueOne(IllustDownloadVM vm)
        {
            if (!QueuedTasks.Contains(vm))
                QueuedTasks.Add(vm);
            CheckStartNew();
        }

        internal void CompleteOne(IllustDownloadVM _)
        {
            PendingDownloads--;
            ActiveDownloads--;
            CheckStartNew();
        }

        public ObservableCollection<IllustDownloadVM> QueuedTasks { get; } = new();

        private void CheckStartNew()
        {
            while (ActiveDownloads < _setting.MaxDownloadsInParallel.Value)
            {
                var task = QueuedTasks.FirstOrDefault(x => x.State == IllustDownloadState.Waiting);
                if (task is null)
                    return;

                ActiveDownloads++;
                task.Start();
            }
        }

        public void Prune()
        {
            for (int i = 0; i < QueuedTasks.Count; i++)
                if (QueuedTasks[i].State is not (IllustDownloadState.Waiting or IllustDownloadState.Active))
                    QueuedTasks.RemoveAt(i--);
        }

        public void Dispose() => _settingSubscriber.Dispose();
    }
}
