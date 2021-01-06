using System;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Pixiv.Services.Download;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Download
{
#pragma warning disable CA1001
    public sealed class DownloadTaskVM : ObservableObject
#pragma warning restore CA1001
    {
        private readonly DownloadManager _downloadManager;
        private readonly DownloadTask _downloadTask;
        private CancellationTokenSource? _cts;

        private readonly SynchronizationContext? _synchronizationContext = SynchronizationContext.Current;

        internal DownloadTaskVM(DownloadManager downloadManager, DownloadTask downloadTask)
        {
            _downloadManager = downloadManager;
            _downloadTask = downloadTask;
        }

        public Illust Illust => _downloadTask.Illust;

        public ImageInfo IllustPreviewPage => Illust.Pages[0].Medium;

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

                    await _downloadTask.RunAsync(cancellationToken: _cts.Token,
                        onProgress: p =>
                        {
                            if (_synchronizationContext is null)
                                Progress = p;
                            else
                                _synchronizationContext.Post(
                                    o => Progress = (double)o!,
                                    p);
                        }).ConfigureAwait(true);

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
            if (_cts is null)
            {
                State = DownloadTaskState.Canceled;
            }
            else
            {
                State = DownloadTaskState.CancelRequested;
                _cts.Cancel();
            }
        }
    }

    public enum DownloadTaskState
    {
        Waiting,
        Active,
        Completed,
        Faulted,
        CancelRequested,
        Canceled
    }
}
