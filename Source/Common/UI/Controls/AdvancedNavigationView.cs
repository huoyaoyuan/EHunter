using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EHunter.Controls
{
    [DependencyProperty("SelectedViewModel", typeof(object), IsNullable = true, ChangedMethod = "SelectedVMChanged")]
    [DependencyProperty("SettingsViewModel", typeof(object), IsNullable = true, ChangedMethod = "SettingsVMChanged")]
    [DependencyProperty("PageLocator", typeof(IPageLocator), IsNullable = true)]
    public partial class AdvancedNavigationView : NavigationView
    {
        private readonly Frame _frame = new();

        public AdvancedNavigationView()
        {
            Content = _frame;

            _frame.RegisterPropertyChangedCallback(Frame.CanGoBackProperty,
                (s, e) => IsBackEnabled = ((Frame)s).CanGoBack);
            SelectionChanged += AdvancedNavigationView_SelectionChanged;
            BackRequested += AdvancedNavigationView_BackRequested;

            RegisterPropertyChangedCallback(SettingsItemProperty, (d, p) =>
            {
                var anv = (AdvancedNavigationView)d;
                if (anv.SettingsItem != null)
                    SetViewModel((NavigationViewItemBase)anv.SettingsItem, anv.SettingsViewModel);
            });
        }

        private static void SettingsVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var anv = (AdvancedNavigationView)d;
            if (anv.SettingsItem != null)
                SetViewModel((NavigationViewItemBase)anv.SettingsItem, anv.SettingsViewModel);
        }

        private static void SelectedVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var anv = (AdvancedNavigationView)d;
            object? newValue = e.NewValue;

            if (newValue is null)
                return;

            foreach (var item in RecurseAllItems(anv))
            {
                if (GetViewModel(item) == newValue)
                {
                    item.IsSelected = true;
                    return;
                }
            }
        }

        private void AdvancedNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is null)
                return;

            object? viewModel = GetViewModel(args.SelectedItemContainer);
            if (viewModel is null)
                return;

            var viewType = PageLocator?.MapPageType(viewModel);
            if (viewType != null && _frame.CurrentSourcePageType != viewType)
                _frame.Navigate(viewType, viewModel);

            SelectedViewModel = viewModel;
        }

        private void AdvancedNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            _frame.GoBack();
            var viewType = _frame.CurrentSourcePageType;

            foreach (var item in RecurseAllItems(this))
            {
                object? viewModel = GetViewModel(item);
                if (viewModel != null && PageLocator?.MapPageType(viewModel) == viewType)
                {
                    item.IsSelected = true;
                    return;
                }
            }
        }

        public static readonly DependencyProperty ViewModelProperty
            = DependencyProperty.RegisterAttached("ViewModel", typeof(object), typeof(NavigationViewItemBase), new PropertyMetadata(null));
        public static void SetViewModel(NavigationViewItemBase dependencyObject, object? value)
            => dependencyObject.SetValue(ViewModelProperty, value);
        public static object? GetViewModel(NavigationViewItemBase dependencyObject)
            => dependencyObject.GetValue(ViewModelProperty);

        private static IEnumerable<NavigationViewItemBase> RecurseAllItems(NavigationView navigationView)
        {
            static IEnumerable<NavigationViewItemBase> Recurse(IEnumerable<object> list)
            {
                foreach (object? obj in list)
                {
                    switch (obj)
                    {
                        case NavigationViewItem item:
                            yield return item;
                            foreach (var inner in Recurse(item.MenuItems))
                                yield return inner;
                            break;
                        case NavigationViewItemBase baseItem:
                            yield return baseItem;
                            break;
                    }
                }
            }

            return Recurse(navigationView.MenuItems
                .Concat(navigationView.FooterMenuItems)
                .Append((NavigationViewItemBase)navigationView.SettingsItem));
        }
    }
}
