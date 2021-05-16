using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#nullable enable

namespace EHunter.Controls
{
    public static class NavigationHelper
    {
        public static readonly DependencyProperty NavigationTypeProperty
            = DependencyProperty.RegisterAttached("NavigationType", typeof(Type), typeof(NavigationViewItemBase),
                new PropertyMetadata(null));
        public static Type GetNavigationType(NavigationViewItemBase dependencyObject)
            => (Type)dependencyObject.GetValue(NavigationTypeProperty);
        public static void SetNavigationType(NavigationViewItemBase dependencyObject, Type value)
            => dependencyObject.SetValue(NavigationTypeProperty, value);

        public static readonly DependencyProperty SettingsViewTypeProperty
            = DependencyProperty.RegisterAttached("SettingsViewType", typeof(Type), typeof(NavigationView),
                new PropertyMetadata(null));
        public static Type GetSettingsViewType(NavigationView dependencyObject)
            => (Type)dependencyObject.GetValue(SettingsViewTypeProperty);
        public static void SetSettingsViewType(NavigationView dependencyObject, Type value)
            => dependencyObject.SetValue(SettingsViewTypeProperty, value);

        public static readonly DependencyProperty IsAutoNavigationEnabledProperty
            = DependencyProperty.RegisterAttached("IsAutoNavigationEnabled", typeof(bool), typeof(NavigationView),
                new PropertyMetadata(false, (d, e) =>
                {
                    var obj = (NavigationView)d;
                    if (e.NewValue is true)
                    {
                        obj.SelectionChanged += NavigationView_SelectionChanged;
                        obj.BackRequested += NavigationView_BackRequested;
                    }
                    else
                    {
                        obj.SelectionChanged -= NavigationView_SelectionChanged;
                        obj.BackRequested -= NavigationView_BackRequested;
                    }

                    static void NavigationView_SelectionChanged(
                        NavigationView sender,
                        NavigationViewSelectionChangedEventArgs args)
                    {
                        if (sender.Content is not Frame frame)
                            return;

                        var type = args.IsSettingsSelected
                            ? GetSettingsViewType(sender)
                            : GetNavigationType(args.SelectedItemContainer);
                        if (type is null || type == frame.CurrentSourcePageType)
                            return;

                        frame.Navigate(type);
                    }

                    static void NavigationView_BackRequested(
                        NavigationView sender,
                        NavigationViewBackRequestedEventArgs args)
                    {

                    }
                }));
        public static bool GetIsAutoNavigationEnabled(NavigationView dependencyObject)
            => (bool)dependencyObject.GetValue(IsAutoNavigationEnabledProperty);
        public static void SetIsAutoNavigationEnabled(NavigationView dependencyObject, bool value)
            => dependencyObject.SetValue(IsAutoNavigationEnabledProperty, value);
    }
}
