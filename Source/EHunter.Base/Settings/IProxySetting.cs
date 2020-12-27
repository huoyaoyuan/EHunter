using System;
using System.Net;

namespace EHunter.Settings
{
    public interface IProxySetting
    {
        IWebProxy? WebProxy { get; }

        IObservable<IWebProxy?> ProxyChanged { get; }
    }
}
