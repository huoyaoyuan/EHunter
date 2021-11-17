using System.Collections.ObjectModel;
using System.Composition;
using DynamicData;
using EHunter.Pixiv.Services.Download;

namespace EHunter.Pixiv.ViewModels.Download
{
    [Export, Shared]
    public sealed class AllDownloadsVM : IDisposable
    {
        private readonly IDisposable _subscription;

        private readonly ReadOnlyObservableCollection<ActiveDownloadVM> _tasks;
        public ReadOnlyObservableCollection<ActiveDownloadVM> Tasks => _tasks;

        [ImportingConstructor]
        public AllDownloadsVM(DownloadManager downloadManager, PixivVMFactory factory)
        {
            _subscription = downloadManager.QueuedTasks
                .Connect()
                .Transform(x => new ActiveDownloadVM(x, factory))
                .Bind(out _tasks)
                .DisposeMany()
                .Subscribe();
        }

        public void Dispose() => _subscription.Dispose();
    }
}
