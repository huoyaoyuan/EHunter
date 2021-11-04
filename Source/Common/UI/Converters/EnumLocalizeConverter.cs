using Microsoft.UI.Xaml.Data;
using Microsoft.Windows.ApplicationModel.Resources;

namespace EHunter.Converters
{
    public sealed class EnumLocalizeConverter : IValueConverter
    {
        private readonly ResourceManager _resourceManager = new();

        public object? Convert(object? value, Type targetType, object? parameter, string language)
        {
            if (value is null)
                return null;

            var resourceMap = _resourceManager.MainResourceMap;
            if (parameter is not null)
                resourceMap = resourceMap.TryGetSubtree(parameter.ToString());

            return resourceMap
                ?.TryGetValue($"{value.GetType().Name}_{value}")
                ?.ValueAsString;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, string language)
            => throw new NotSupportedException();
    }
}
