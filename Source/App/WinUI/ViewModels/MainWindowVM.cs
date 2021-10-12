using System.Collections.Immutable;
using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.Providers;

namespace EHunter.UI.ViewModels
{
    [Export]
    public class MainWindowVM : ObservableObject
    {
        [ImportingConstructor]
        public MainWindowVM([ImportMany] IEnumerable<IEHunterProvider> providers)
        {
            Providers = providers.ToImmutableArray();
            TopNavigationEntries = Providers.Select(x => new IconNavigationEntry
            {
                Title = x.Title,
                ViewType = x.UIRootType,
                IconUri = x.IconUri
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
