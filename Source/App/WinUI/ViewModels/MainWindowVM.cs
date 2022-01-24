using System.Collections.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.Providers;

namespace EHunter.UI.ViewModels
{
    public partial class MainWindowVM : ObservableObject
    {
        public MainWindowVM(IServiceProvider serviceProvider, IEnumerable<IEHunterProvider> providers, CommonSettingVM settings)
        {
            Providers = providers.ToImmutableArray();
            TopNavigationEntries = Providers.Select(x => new IconNavigationEntry
            {
                Title = x.Title,
                UIRoot = x.CreateUIRoot(serviceProvider),
                IconUri = x.IconUri
            }).ToArray();
            Settings = settings;
        }

        public ImmutableArray<IEHunterProvider> Providers { get; }

        public IReadOnlyList<NavigationEntry> TopNavigationEntries { get; }

        [ObservableProperty]
        private NavigationEntry? _selectedEntry;

        public CommonSettingVM Settings { get; }
    }
}
