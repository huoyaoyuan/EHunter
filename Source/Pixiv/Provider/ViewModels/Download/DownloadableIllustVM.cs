using System;
using System.Threading.Tasks;
using System.Windows.Input;
using EHunter.Pixiv.Services.Download;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Download
{
    public class DownloadableIllustVM : ObservableObject, ICommand
    {
        private readonly DownloadManager _manager;

        public DownloadableIllustVM(DownloadManager manager, Illust illust, DownloadTaskVM? existingDownload, ValueTask<DownloadableState> canDownload)
        {
            _manager = manager;
            Illust = illust;
            DownloadTask = existingDownload;
            AwaitCandownload();

            async void AwaitCandownload()
                => State = await canDownload.ConfigureAwait(true);
        }

        public Illust Illust { get; }

        private static readonly EventArgs s_eventArgs = new();

        private DownloadableState _state;
        public DownloadableState State
        {
            get => _state;
            private set
            {
                if (SetProperty(ref _state, value))
                    CanExecuteChanged?.Invoke(this, s_eventArgs);
            }
        }

        private DownloadTaskVM? _downloadTask;
        public DownloadTaskVM? DownloadTask
        {
            get => _downloadTask;
            private set => SetProperty(ref _downloadTask, value);
        }

        public bool CanExecute(object? parameter) => State == DownloadableState.CanDownload && DownloadTask is null;
        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                throw new InvalidOperationException("Called at wrong status.");

            State = DownloadableState.AlreadyPending;
            DownloadTask = await _manager.CreateDownloadTaskAsync(Illust).ConfigureAwait(true);
        }

        public event EventHandler? CanExecuteChanged;
    }
}
