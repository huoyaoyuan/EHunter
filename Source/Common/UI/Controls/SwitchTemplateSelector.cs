using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace EHunter.Controls
{
    [ContentProperty(Name = nameof(Cases))]
    public abstract class SwitchTemplateSelector : DataTemplateSelector
    {
        protected abstract object? MapValue(object? value);

        public Collection<SelectorCase> Cases { get; } = new();

        protected override DataTemplate? SelectTemplateCore(object? item)
        {
            object? mappedValue = MapValue(item);

            foreach (var c in Cases)
            {
                if (Equals(mappedValue, c.Value))
                    return c.DataTemplate;
            }

            return null;
        }

        protected override DataTemplate? SelectTemplateCore(object? item, DependencyObject? container) => SelectTemplateCore(item);
    }

    [ContentProperty(Name = nameof(DataTemplate))]
    public class SelectorCase
    {
        public object? Value { get; set; }
        public DataTemplate? DataTemplate { get; set; }
    }
}
