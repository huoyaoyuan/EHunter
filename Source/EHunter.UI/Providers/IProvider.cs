using System;

namespace EHunter.UI.Providers
{
    public interface IProvider
    {
        public string Name { get; }
        public Uri IconUri { get; }
    }
}
