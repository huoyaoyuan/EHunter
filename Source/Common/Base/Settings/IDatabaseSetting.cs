using System;

namespace EHunter.Settings
{
    public interface IDatabaseSetting
    {
        public string ConnectionString { get; }
        public IObservable<string> ConnectionStringChanged { get; }
    }
}
