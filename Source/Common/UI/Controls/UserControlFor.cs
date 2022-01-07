using EHunter.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EHunter.Controls
{
    public abstract class UserControlFor<TViewModel> : UserControl, IViewFor<TViewModel>
        where TViewModel : class
    {
        public static readonly DependencyProperty ViewModelProperty
            = DependencyProperty.Register(nameof(ViewModel), typeof(TViewModel), typeof(PageFor<TViewModel>),
                new PropertyMetadata(null, (s, e) =>
                {
                    var control = (UserControl)s;
                    if (control.IsLoaded)
                    {
                        if (e.OldValue is IActivatable oldActivatable)
                        {
                            oldActivatable.OnDeactivated();
                        }
                        if (e.NewValue is IActivatable newActivatable)
                        {
                            newActivatable.OnActivated();
                        }
                    }
                }));
        public TViewModel? ViewModel
        {
            get => (TViewModel?)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        protected UserControlFor()
        {
            Loaded += (s, e) =>
            {
                if (ViewModel is IActivatable activatable)
                {
                    activatable.OnActivated();
                }
            };
            Unloaded += (s, e) =>
            {
                if (ViewModel is IActivatable activatable)
                {
                    activatable.OnDeactivated();
                }
            };
        }
    }
}
