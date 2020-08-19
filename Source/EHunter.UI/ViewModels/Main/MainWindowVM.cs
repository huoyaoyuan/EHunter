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
            NavigationCommands = new List<NavigationCommandItem>
            {
                new NavigationCommandItem("Recent", "\uE71D")
                {
                    SubItems = providers.Select(p => new NavigationCommandItem(p.Name, string.Empty)).ToList()
                },
                new NavigationCommandItem("Search", "\uE721")
                {
                    SubItems = providers.Select(p => new NavigationCommandItem(p.Name, string.Empty)).ToList()
                },
                new NavigationCommandItem("Opened Galleries", "\uE736"),
                new NavigationCommandItem("Pinned Pictures", "\uE718"),
            };
        }

        public List<NavigationCommandItem> NavigationCommands { get; }
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

        public List<NavigationCommandItem>? SubItems { get; set; }
    }
}
