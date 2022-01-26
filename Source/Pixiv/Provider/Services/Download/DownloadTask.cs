using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Meowtrix.PixivApi.Models;
using static EHunter.Pixiv.Services.Download.IllustDownloadState;

namespace EHunter.Pixiv.Services.Download
{
#pragma warning disable CA1001
    public partial class DownloadTask : ObservableObject
#pragma warning restore CA1001
    {
        private readonly DownloadManager _downloadManager;
        private readonly SynchronizationContext? _synchronizationContext = SynchronizationContext.Current;
        private CancellationTokenSource? _cts;

        [ObservableProperty]
        private Exception? _exception;

        [ObservableProperty]
        private double _progress;

        public DownloadTask(Illust illust, DownloadManager downloadManager)
        {
            Illust = illust;
            _downloadManager = downloadManager;

            CheckDownloadable();

            async void CheckDownloadable()
            {
                var downloadable = await _downloadManager.Downloader.CanDownloadAsync(Illust.Id)
                    .ConfigureAwait(true);

                if (State == NotLoaded)
                {
                    State = (downloadable switch
                    {
                        DownloadableState.AlreadyDownloaded => Completed,
                        _ => Idle
                    });
                }
            }
        }

        internal async void Start()
        {
            State = Active;

            _cts = new CancellationTokenSource();
            using (_cts)
            {
                bool shouldRemovePending = true;

                try
                {
                    await _downloadManager.Downloader.DownloadAsync(
                        Illust,
                        onProgress: p =>
                        {
                            if (_synchronizationContext is null)
                                Progress = p;
                            else
                                _synchronizationContext.Post(
                                    o => Progress = (double)o!,
                                    p);
                        },
                        cancellationToken: _cts.Token)
                        .ConfigureAwait(true);

                    State = Completed;
                }
                catch (TaskCanceledException tce) when (tce.InnerException is not TimeoutException)
                {
                    State = Canceled;
                }
                catch (Exception ex)
                {
                    shouldRemovePending = false;
                    Exception = ex;
                    State = Faulted;
                }
                finally
                {
                    if (shouldRemovePending)
                        await _downloadManager.Downloader.RemoveFromPendingAsync(Illust.Id).ConfigureAwait(true);
                    _downloadManager.CompleteOne(this);
                }
            }
            _cts = null;
        }

        public Illust Illust { get; }

        [ICommand(CanExecute = nameof(CanDownload), AllowConcurrentExecutions = false)]
        private async Task Download()
        {
            bool shoudAddPending = State switch
            {
                Idle => true,
                Canceled => true,
                Faulted => false,
                _ => throw new InvalidOperationException($"{nameof(Download)} shouldn't be called at state {State}.")
            };
            switch (await _downloadManager.Downloader.CanDownloadAsync(Illust.Id).ConfigureAwait(true))
            {
                case DownloadableState.CanDownload:
                    break;

                case DownloadableState.AlreadyDownloaded:
                    State = Completed;
                    return;

                case DownloadableState.AlreadyPending:
                    if (shoudAddPending)
                        return;
                    break;

                case DownloadableState.ServiceUnavailable:
                    return;

                default:
                    throw new InvalidOperationException("New state should be handled here.");
            }

            if (shoudAddPending)
                await _downloadManager.Downloader.AddToPendingAsync(Illust.Id).ConfigureAwait(true);

            Progress = 0;
            SetWaiting();

            _downloadManager.QueueOne(this);
        }

        private bool CanDownload() => State is Idle or Faulted or Canceled;

        internal void SetWaiting() => State = Waiting;

        [ICommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
        {
            if (_cts is null)
                State = Canceled;
            else
            {
                State = CancelRequested;
                _cts.Cancel();
            }
        }

        private bool CanCancel() => State is Waiting or Active;

        [ObservableProperty]
        [AlsoNotifyCanExecuteFor(nameof(DownloadCommand))]
        [AlsoNotifyCanExecuteFor(nameof(CancelCommand))]
        private IllustDownloadState _state;
    }

    public enum IllustDownloadState
    {
        NotLoaded,
        Idle,
        Waiting,
        Active,
        Completed,
        CancelRequested,
        Canceled,
        Faulted,
    }
}
