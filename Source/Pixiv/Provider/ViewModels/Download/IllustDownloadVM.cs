using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using EHunter.Pixiv.Services.Download;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using static EHunter.Pixiv.ViewModels.Download.IllustDownloadState;

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
#pragma warning disable CA1508 // false positive
            using (_cts)
#pragma warning restore CA1508
            {
                bool shouldRemovePending = true;

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

                    State = Completed;
                }
                catch (TaskCanceledException)
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

        private readonly ActionCommand _downloadCommand;
        public ICommand DownloadCommand => _downloadCommand;
        private async void Download()
        {
            bool shoudAddPending = State switch
            {
                Idle => true,
                Canceled => true,
                Faulted => false,
                _ => throw new InvalidOperationException($"{nameof(Download)} shouldn't be called at state {State}.")
            };

            Progress = 0;
            SetWaiting();

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

            _downloadManager.QueueOne(this);
        }

        internal void SetWaiting() => State = Waiting;

        private readonly ActionCommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand;
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

        private IllustDownloadState _state;
        public IllustDownloadState State
        {
            get => _state;
            private set
            {
                if (SetProperty(ref _state, value))
                {
                    _downloadCommand.SetCanExecute(value is Idle or Faulted or Canceled);
                    _cancelCommand.SetCanExecute(value is Waiting or Active);
                }
            }
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
        NotLoaded,
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
