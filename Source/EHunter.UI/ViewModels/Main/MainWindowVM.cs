using System.Collections.Generic;
using System.Linq;
using EHunter.UI.Providers;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#pragma warning disable CA2227

namespace EHunter.UI.ViewModels.Main
{
    public class MainWindowVM
    {
        public MainWindowVM(IEnumerable<IProvider> providers)
        {
            NavigationCommands = new[]
            {
                new NavigationCommandItem("Recent", "\uE71D")
                {
                    SubItems = providers.Select(p => new ProviderSpecificCommandItem(p, "Recent")).ToArray()
                },
                new NavigationCommandItem("Search", "\uE721")
                {
                    SubItems = providers.Select(p => new ProviderSpecificCommandItem(p, "Search")).ToArray()
                },
                new NavigationCommandItem("Opened Galleries", "\uE736"),
                new NavigationCommandItem("Pinned Pictures", "\uE718"),
            };
        }

        public IEnumerable<ObservableObject> NavigationCommands { get; }
    }

    public class NavigationCommandItem : ObservableObject
    {
        public NavigationCommandItem(string id, string glyph)
        {
            Id = id;
            IconGlyph = glyph;
            _title = id;
        }

        public string Id { get; }

        public string IconGlyph { get; }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public IEnumerable<ObservableObject>? SubItems { get; set; }
    }

    public class ProviderSpecificCommandItem : ObservableObject
    {
        public ProviderSpecificCommandItem(IProvider provider, string commandType)
        {
            Provider = provider;
            CommandType = commandType;
        }

        public IProvider Provider { get; }
        public string CommandType { get; }
    }
}
