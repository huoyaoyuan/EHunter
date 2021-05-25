using System;
using EHunter.UI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EHunter.UI.Views
{
    internal class NavigationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? IconTemplate { get; set; }
        public DataTemplate? GlyphTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item)
            => item switch
            {
                IconNavigationEntry => IconTemplate,
                GlyphNavigationEntry => GlyphTemplate,
                null => null,
                _ => throw new InvalidOperationException("Unexpected type")
            };

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }
}
