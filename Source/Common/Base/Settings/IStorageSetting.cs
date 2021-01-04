using System;
using System.IO;

namespace EHunter.Settings
{
    public interface IStorageSetting
    {
        DirectoryInfo? StorageRoot { get; }

        IObservable<DirectoryInfo?> StorageChanged { get; }
    }
}
