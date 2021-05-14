using EHunter.ComponentModel;
using EHunter.Settings;

namespace EHunter.Pixiv.Downloader.Manual
{
    internal class DummyStorageSetting : IStorageSetting
    {
        public DummyStorageSetting(string storageRoot) => StorageRoot = new(storageRoot);

        public ObservableProperty<string?> StorageRoot { get; }
    }
}
