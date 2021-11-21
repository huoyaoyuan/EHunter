using EHunter.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace EHunter.Controls
{
    public abstract class PageFor<TViewModel> : Page, IViewFor<TViewModel>
        where TViewModel : class
    {
        public static readonly DependencyProperty ViewModelProperty
            = DependencyProperty.Register(nameof(ViewModel), typeof(TViewModel), typeof(PageFor<TViewModel>),
                new PropertyMetadata(null));
        public TViewModel? ViewModel
        {
            get => (TViewModel?)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is TViewModel viewModel)
                ViewModel = viewModel;

            if (ViewModel is IActivatable activatable)
                activatable.OnActivated();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (ViewModel is IActivatable activatable)
                activatable.OnDeactivated();
        }
    }
}
