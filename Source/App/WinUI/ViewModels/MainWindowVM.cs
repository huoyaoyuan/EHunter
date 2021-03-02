using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using EHunter.Providers;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.UI.ViewModels
{
    [Export]
    public class MainWindowVM : ObservableObject
    {
        [ImportingConstructor]
        public MainWindowVM([ImportMany] IEnumerable<IEHunterProvider> providers)
        {
            Providers = providers.ToImmutableArray();
            TopNavigationEntries = Providers.Select(x => new NavigationEntry
            {
                Title = x.Title,
                ViewType = x.UIRootType
            }).ToArray();
        }

        public ImmutableArray<IEHunterProvider> Providers { get; }

        public IReadOnlyList<NavigationEntry> TopNavigationEntries { get; }

        private NavigationEntry? _selectedEntry;
        public NavigationEntry? SelectedEntry
        {
            get => _selectedEntry;
            set => SetProperty(ref _selectedEntry, value);
        }
    }
}
