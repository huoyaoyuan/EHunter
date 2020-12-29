using System;
using System.IO;
using EHunter.Settings;

namespace EHunter.Pixiv.Downloader.Manual
{
    internal class DummyStorageSetting : IStorageSetting, IObservable<DirectoryInfo?>
    {
        public DummyStorageSetting(string storageRoot) => StorageRoot = new DirectoryInfo(storageRoot);

        public DirectoryInfo? StorageRoot { get; }
        public IObservable<DirectoryInfo?> StorageChanged => this;

        public IDisposable Subscribe(IObserver<DirectoryInfo?> observer) => new DummyDisposable();

        private class DummyDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}
