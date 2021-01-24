using EHunter.ComponentModel;

namespace EHunter.Settings
{
    public interface IDatabaseSetting
    {
        public ObservableProperty<string> ConnectionString { get; }
    }
}
