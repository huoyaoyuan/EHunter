using System.Net;
using EHunter.ComponentModel;

namespace EHunter.Settings
{
    public interface IProxySetting
    {
        ObservableProperty<IWebProxy?> Proxy { get; }
    }
}
