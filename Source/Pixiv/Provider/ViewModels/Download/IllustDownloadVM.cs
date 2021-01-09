using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using EHunter.Pixiv.Services.Download;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Download
{
#pragma warning disable CA1001
    public class IllustDownloadVM : ObservableObject
#pragma warning restore CA1001
    {
        private readonly DownloadManager _downloadManager;
        private readonly SynchronizationContext? _synchronizationContext = SynchronizationContext.Current;
        private CancellationTokenSource? _cts;

        public IllustDownloadVM(Illust illust, DownloadManager downloadManager)
        {
            Illust = illust;
            _downloadManager = downloadManager;
            _downloadCommand = new(Download);
            _cancelCommand = new(Cancel);

            CheckDownloadable();

            async void CheckDownloadable()
            {
                (var state, bool canDownload) = await _downloadManager.Downloader.CanDownloadAsync(Illust.Id)
                    .ConfigureAwait(true) switch
                {
                    DownloadableState.AlreadyDownloaded => (IllustDownloadState.Completed, false),
                    _ => (IllustDownloadState.Idle, true)
                };

                State = state;
                _downloadCommand.SetCanExecute(canDownload);
            }
        }

        internal async void Start()
        {
            State = IllustDownloadState.Active;

            _cts = new CancellationTokenSource();
#pragma warning disable CA1508 // false positive
            using (_cts)
#pragma warning restore CA1508
            {
                try
                {
                    var task = _downloadManager.Downloader.CreateDownloadTask(Illust);

                    await task.RunAsync(cancellationToken: _cts.Token,
                        onProgress: p =>
                        {
                            if (_synchronizationContext is null)
                                Progress = p;
                            else
                                _synchronizationContext.Post(
                                    o => Progress = (double)o!,
                                    p);
                        }).ConfigureAwait(true);

                    State = IllustDownloadState.Completed;
                }
                catch (TaskCanceledException)
                {
                    State = IllustDownloadState.Canceled;
                }
                catch (Exception ex)
                {
                    Exception = ex;
                    State = IllustDownloadState.Faulted;
                }
                finally
                {
                    await _downloadManager.Downloader.RemoveFromPendingAsync(Illust.Id).ConfigureAwait(true);
                    _downloadManager.CompleteOne(this);
                }
            }
            _cts = null;
        }

        public Illust Illust { get; }

        private readonly ActionCommand _downloadCommand;
        public ICommand DownloadCommand => _downloadCommand;
        private async void Download()
        {
            if (await _downloadManager.Downloader.CanDownloadAsync(Illust.Id).ConfigureAwait(true)
                == DownloadableState.CanDownload)
            {
                SetQueued();

                await _downloadManager.Downloader.AddToPendingAsync(Illust.Id).ConfigureAwait(true);
                _downloadManager.QueueOne(this);
            }
        }

        internal void SetQueued()
        {
            State = IllustDownloadState.Waiting;
            _cancelCommand.SetCanExecute(true);
            _downloadCommand.SetCanExecute(false);
        }

        private readonly ActionCommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand;
        private void Cancel()
        {
            _cancelCommand.SetCanExecute(false);

            if (_cts is null)
                State = IllustDownloadState.Canceled;
            else
            {
                _cts.Cancel();
                State = IllustDownloadState.CancelRequested;
            }
        }

        private IllustDownloadState _state;
        public IllustDownloadState State
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

        private double _progress;
        public double Progress
        {
            get => _progress;
            private set => SetProperty(ref _progress, value);
        }

        public ImageInfo IllustPreviewPage => Illust.Pages[0].Medium;
    }

    public enum IllustDownloadState
    {
        Idle,
        Waiting,
        Active,
        Completed,
        CancelRequested,
        Canceled,
        Faulted,
    }

    internal class ActionCommand : ICommand
    {
        private readonly Action _action;
        private bool _canExecute;

        public ActionCommand(Action action) => _action = action;

        public bool CanExecute(object? parameter) => _canExecute;
        public void Execute(object? parameter) => _action();

        private static readonly EventArgs s_eventArgs = new();
        public event EventHandler? CanExecuteChanged;

        public void SetCanExecute(bool canExecute)
        {
            _canExecute = canExecute;
            CanExecuteChanged?.Invoke(this, s_eventArgs);
        }
    }
}
