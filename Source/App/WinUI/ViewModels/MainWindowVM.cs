using System.Collections.Immutable;
using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.Providers;

namespace EHunter.UI.ViewModels
{
    [Export]
    public partial class MainWindowVM : ObservableObject
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

        [ObservableProperty]
        private NavigationEntry? _selectedEntry;
    }
}
