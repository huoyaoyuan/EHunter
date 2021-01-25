using System.IO;
using EHunter.ComponentModel;
using EHunter.Settings;

namespace EHunter.Pixiv.Downloader.Manual
{
    internal class DummyStorageSetting : IStorageSetting
    {
        public DummyStorageSetting(string storageRoot) => StorageRoot = new(new DirectoryInfo(storageRoot));

        public ObservableProperty<DirectoryInfo?> StorageRoot { get; }
    }
}
