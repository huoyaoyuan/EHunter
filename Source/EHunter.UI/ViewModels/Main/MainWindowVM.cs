using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#pragma warning disable CA2227

namespace EHunter.UI.ViewModels.Main
{
    public class MainWindowVM
    {
        public List<NavigationCommandItem> NavigationCommands { get; } = new List<NavigationCommandItem>
        {
            new NavigationCommandItem("Recent", "\uE71D")
            {
                SubItems = new List<NavigationCommandItem>
                {
                    new NavigationCommandItem("EHentai", string.Empty),
                    new NavigationCommandItem("Pixiv", string.Empty),
                }
            },
            new NavigationCommandItem("Search", "\uE721")
            {
                SubItems = new List<NavigationCommandItem>
                {
                    new NavigationCommandItem("EHentai", string.Empty),
                    new NavigationCommandItem("Pixiv", string.Empty),
                }
            },
            new NavigationCommandItem("Opened Galleries", "\uE736"),
            new NavigationCommandItem("Pinned Pictures", "\uE718"),
        };
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
