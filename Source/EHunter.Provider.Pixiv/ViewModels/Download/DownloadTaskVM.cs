using System;
using System.Threading;
using EHunter.Provider.Pixiv.Services.Download;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels.Download
{
    public sealed class DownloadTaskVM : ObservableObject, IObserver<double>
    {
        private double _progress;
        private readonly DownloadManager _downloadManager;
        private readonly DownloadTask _downloadTask;

        private readonly SynchronizationContext? _executionContext = SynchronizationContext.Current;
        private IDisposable? _subscriber;

        internal DownloadTaskVM(DownloadManager downloadManager, DownloadTask downloadTask)
        {
            _downloadManager = downloadManager;
            _downloadTask = downloadTask;
        }

        public Illust Illust => _downloadTask.Illust;

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

        internal void Start()
        {
            _subscriber = _downloadTask.Progress.Subscribe(this);
            _downloadTask.Start();
        }

        void IObserver<double>.OnCompleted()
        {
            if (_executionContext is null)
                State = DownloadTaskState.Completed;
            else
                _executionContext.Post(_ => State = DownloadTaskState.Completed, null);
            _subscriber?.Dispose();

            _downloadManager.CompleteOne(this);
        }

        void IObserver<double>.OnError(Exception error)
        {
            if (_executionContext is null)
            {
                Exception = error;
                State = DownloadTaskState.Faulted;
            }
            else
            {
                _executionContext.Post(o =>
                  {
                      Exception = (Exception)o!;
                      State = DownloadTaskState.Faulted;
                  }, error);
            }
            _subscriber?.Dispose();

            _downloadManager.CompleteOne(this);
        }

        void IObserver<double>.OnNext(double value)
        {
            if (_executionContext is null)
                Progress = value;
            else
                _executionContext.Post(p => Progress = (double)p!, value);
        }
    }

    public enum DownloadTaskState
    {
        Waiting,
        Active,
        Completed,
        Faulted
    }
}
