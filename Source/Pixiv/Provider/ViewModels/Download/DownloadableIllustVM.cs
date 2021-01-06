using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Download
{
    public class DownloadableIllustVM : ObservableObject, ICommand
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
        internal bool CanDownload
        {
            get => _canDownload;
            private set
            {
                if (SetProperty(ref _canDownload, value))
                    CanExecuteChanged?.Invoke(this, new());
            }
        }

        private DownloadTaskVM? _downloadTask;
        public DownloadTaskVM? DownloadTask
        {
            get => _downloadTask;
            private set => SetProperty(ref _downloadTask, value);
        }

        internal async void Download()
        {
            if ((CanDownload, DownloadTask) != (true, null))
                throw new InvalidOperationException("Called at wrong status.");

            CanDownload = false;
            DownloadTask = await _manager.CreateDownloadTaskAsync(Illust).ConfigureAwait(true);
        }

        public bool CanExecute(object? parameter) => CanDownload;
        public void Execute(object? parameter) => Download();

        public event EventHandler? CanExecuteChanged;
    }
}
