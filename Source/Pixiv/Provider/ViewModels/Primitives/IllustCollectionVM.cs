using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.ComponentModel;
using EHunter.Pixiv.ViewModels.Illusts;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Primitives
{
    public abstract partial class IllustCollectionVM : ObservableObject, IActivatable
    {
        private readonly PixivVMFactory _factory;

        // TODO: Custom source generation
        private AgeRestriction _selectedAge = AgeRestriction.All;
        public AgeRestriction SelectedAge
        {
            get => _selectedAge;
            set
            {
                if (SetProperty(ref _selectedAge, value) && _loaded && AutoRefresh)
                    Refresh();
            }
        }

        protected virtual bool AutoRefresh => true;

        [ObservableProperty]
        private IBindableCollection<IllustVM>? _illusts;

        protected IllustCollectionVM(PixivVMFactory factory) => _factory = factory;

        protected abstract IAsyncEnumerable<Illust>? LoadIllusts();

        public void Refresh()
        {
            Debug.Assert(_loaded);

            Illusts = _factory.CreateAsyncCollection(
                LoadIllusts()?.Age(SelectedAge));

            // TODO: Consider AdvancedCollectionView.Filter
            // Currently doesn't work with mignon/IsR18=true
            // 8.0.0-preview2
        }

        private bool _loaded;
        public void OnActivated()
        {
            if (!_loaded)
            {
                _loaded = true;

                if (AutoRefresh)
                    Refresh();
            }
        }

        public void OnDeactivated() { }
    }
}
