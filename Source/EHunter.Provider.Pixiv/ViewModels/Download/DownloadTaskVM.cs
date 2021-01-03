using System;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Provider.Pixiv.Services.Download;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels.Download
{
#pragma warning disable CA1001
    public sealed class DownloadTaskVM : ObservableObject
#pragma warning restore CA1001
    {
        private readonly DownloadManager _downloadManager;
        private readonly DownloadTask _downloadTask;
        private CancellationTokenSource? _cts;

        internal DownloadTaskVM(DownloadManager downloadManager, DownloadTask downloadTask)
        {
            _downloadManager = downloadManager;
            _downloadTask = downloadTask;
        }

        public Illust Illust => _downloadTask.Illust;

        private double _progress;
        public double Progress
        {
            get => _progress;
            private set => SetProperty(ref _progress, value);
        }

        private DownloadTaskState _state;
        public DownloadTaskState State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }

        private Exception? _exception;
        public Exception? Exception
        {
            get => _exception;
            private set => SetProperty(ref _exception, value);
        }

        internal async void Start()
        {
            if (State != DownloadTaskState.Waiting)
                throw new InvalidOperationException("A task can only be started once.");

            _cts = new CancellationTokenSource();
#pragma warning disable CA1508 // false positive
            using (_cts)
#pragma warning restore CA1508
            {
                try
                {
                    State = DownloadTaskState.Active;
#pragma warning disable CA1508 // false positive
                    await foreach (double p in _downloadTask.Start(cancellationToken: _cts.Token).ConfigureAwait(true))
#pragma warning restore CA1508
                        Progress = p;
                    State = DownloadTaskState.Completed;
                }
                catch (TaskCanceledException)
                {
                    State = DownloadTaskState.Canceled;
                }
                catch (Exception ex)
                {
                    Exception = ex;
                    State = DownloadTaskState.Faulted;
                }
                finally
                {
                    _downloadManager.CompleteOne(this);
                }
            }
            _cts = null;
        }

        public void Cancel()
        {
            _cts?.Cancel();
            State = DownloadTaskState.Canceled;
        }
    }

    public enum DownloadTaskState
    {
        Waiting,
        Active,
        Completed,
        Faulted,
        Canceled
    }
}
