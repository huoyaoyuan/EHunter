using EHunter.Services;
using Windows.Storage;

namespace EHunter.UI.Services
{
    internal class WinRTSettingsStore : ISettingsStore
    {
        private readonly ApplicationDataContainer _container;

        public WinRTSettingsStore()
            : this(ApplicationData.Current.LocalSettings)
        {
        }

        private WinRTSettingsStore(ApplicationDataContainer container)
            => _container = container;

        public void ClearValue(string name) => _container.Values.Remove(name);
        public ISettingsStore GetSubContainer(string name)
            => new WinRTSettingsStore(_container.CreateContainer(name, ApplicationDataCreateDisposition.Always));

        public int? GetIntValue(string name) => _container.Values[name] as int?;
        public void SetIntValue(string name, int value) => _container.Values[name] = value;

        public string? GetStringValue(string name) => _container.Values[name] as string;
        public void SetStringValue(string name, string? value) => _container.Values[name] = value;

        public bool? GetBoolValue(string name) => _container.Values[name] as bool?;
        public void SetBoolValue(string name, bool value) => _container.Values[name] = value;
    }
}
