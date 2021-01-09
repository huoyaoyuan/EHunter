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
    public class DownloadableVM2 : ObservableObject
#pragma warning restore CA1001
    {
        private readonly DownloadManager _downloadManager;
        private readonly SynchronizationContext? _synchronizationContext = SynchronizationContext.Current;
        private CancellationTokenSource? _cts;

        public DownloadableVM2(Illust illust, DownloadManager downloadManager)
        {
            Illust = illust;
            _downloadManager = downloadManager;
            _downloadCommand = new(Download);

            CheckDownloadable();

            async void CheckDownloadable()
            {
                (var state, bool canDownload) = await _downloadManager.Downloader.CanDownloadAsync(Illust.Id)
                    .ConfigureAwait(true) switch
                {
                    DownloadableState.AlreadyDownloaded => (DownloadableState2.Completed, false),
                    _ => (DownloadableState2.Idle, true)
                };

                State = state;
                _downloadCommand.SetCanExecute(canDownload);
            }
        }

        internal async void Start()
        {
            State = DownloadableState2.Active;

            _cts = new CancellationTokenSource();
#pragma warning disable CA1508 // false positive
            using (_cts)
#pragma warning restore CA1508
            {
                try
                {
                    var task = await _downloadManager.Downloader.CreateDownloadTaskAsync(Illust).ConfigureAwait(true);

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

                    State = DownloadableState2.Completed;
                }
                catch (TaskCanceledException)
                {
                    State = DownloadableState2.Canceled;
                }
                catch (Exception ex)
                {
                    Exception = ex;
                    State = DownloadableState2.Faulted;
                }
                finally
                {
                    _downloadManager.CompleteOne2(this);
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
                State = DownloadableState2.Waiting;
                _downloadManager.QueueOne2(this);
                _downloadCommand.SetCanExecute(false);
            }
        }

        private DownloadableState2 _state;
        public DownloadableState2 State
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

    public enum DownloadableState2
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
