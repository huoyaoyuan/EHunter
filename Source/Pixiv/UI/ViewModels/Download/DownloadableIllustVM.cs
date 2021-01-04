using System;
using System.Threading.Tasks;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Pixiv.ViewModels.Download
{
    public class DownloadableIllustVM : ObservableObject
    {
        private readonly DownloadManager _manager;

        public DownloadableIllustVM(DownloadManager manager, Illust illust, DownloadTaskVM? existingDownload, ValueTask<bool?> canDownload)
        {
            _manager = manager;
            Illust = illust;
            DownloadTask = existingDownload;
            AwaitCandownload();

            async void AwaitCandownload()
                => CanDownload = (await canDownload.ConfigureAwait(true)) ?? false;
        }

        public Illust Illust { get; }

        private bool _canDownload;
        public bool CanDownload
        {
            get => _canDownload;
            private set => SetProperty(ref _canDownload, value);
        }

        private DownloadTaskVM? _downloadTask;
        public DownloadTaskVM? DownloadTask
        {
            get => _downloadTask;
            private set => SetProperty(ref _downloadTask, value);
        }

        public async void Download()
        {
            if ((CanDownload, DownloadTask) != (true, null))
                throw new InvalidOperationException("Called at wrong status.");

            CanDownload = false;
            DownloadTask = await _manager.CreateDownloadTaskAsync(Illust).ConfigureAwait(true);
        }
    }
}
