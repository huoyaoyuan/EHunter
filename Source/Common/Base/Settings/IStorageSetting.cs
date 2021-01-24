using System.IO;
using EHunter.ComponentModel;

namespace EHunter.Settings
{
    public interface IStorageSetting
    {
        ObservableProperty<DirectoryInfo?> StorageRoot { get; }
    }
}
