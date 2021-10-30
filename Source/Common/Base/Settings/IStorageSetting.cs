using EHunter.ComponentModel;

namespace EHunter.Settings
{
    public interface IStorageSetting
    {
        ObservableProperty<string?> StorageRoot { get; }
    }
}
