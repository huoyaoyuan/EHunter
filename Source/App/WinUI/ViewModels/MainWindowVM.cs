using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using EHunter.Providers;

#nullable enable

namespace EHunter.UI.ViewModels
{
    [Export]
    public class MainWindowVM
    {
        [ImportingConstructor]
        public MainWindowVM([ImportMany] IEnumerable<IEHunterProvider> providers)
            => Providers = providers.ToImmutableArray();

        public ImmutableArray<IEHunterProvider> Providers { get; }
    }
}
