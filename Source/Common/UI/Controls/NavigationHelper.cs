using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#nullable enable

namespace EHunter.Controls
{
    public static class NavigationHelper
    {
        public static readonly DependencyProperty NavigationTypeProperty
            = DependencyProperty.RegisterAttached("NavigationType", typeof(Type), typeof(NavigationViewItem),
                new PropertyMetadata(null));
        public static Type GetNavigationType(NavigationViewItem dependencyObject)
            => (Type)dependencyObject.GetValue(NavigationTypeProperty);
        public static void SetNavigationType(NavigationViewItem dependencyObject, Type value)
            => dependencyObject.SetValue(NavigationTypeProperty, value);
    }
}
