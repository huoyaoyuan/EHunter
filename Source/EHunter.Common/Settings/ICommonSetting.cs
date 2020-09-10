using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace EHunter.Settings
{
    public interface ICommonSetting : INotifyPropertyChanged
    {
        string StorageRoot { get; set; }
        DirectoryInfo StorageRootDirectory { get; }
        string ProxyAddress { get; set; }
        int ProxyPort { get; set; }
        IWebProxy? Proxy { get; }
        string DbConnectionString { get; set; }
        event Action<IWebProxy?> ProxyUpdated;
    }
}
