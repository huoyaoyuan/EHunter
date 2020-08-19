using EHunter.UI.ViewModels.Main;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EHunter.UI.Views
{
    public class MainNavigationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? TopLevelTemplate { get; set; }
        public DataTemplate? ProviderLevelTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);

        protected override DataTemplate? SelectTemplateCore(object item)
            => item is ProviderSpecificCommandItem
            ? ProviderLevelTemplate
            : TopLevelTemplate;
    }
}
