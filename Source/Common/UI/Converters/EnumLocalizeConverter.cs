using Microsoft.UI.Xaml.Data;
using Microsoft.Windows.ApplicationModel.Resources;

namespace EHunter.Converters
{
    public sealed class EnumLocalizeConverter : IValueConverter
    {
        private readonly ResourceManager _resourceManager = new();
        private ResourceMap? _resourceMap;

        private string? _scope;
        public string? Scope
        {
            get => _scope;
            set
            {
                if (_scope != value)
                {
                    _scope = value;

                    _resourceMap = string.IsNullOrEmpty(value)
                        ? _resourceManager.MainResourceMap
                        : _resourceManager.MainResourceMap.TryGetSubtree(value);
                }
            }
        }

        public EnumLocalizeConverter()
        {
            _resourceManager = new();
            _resourceMap = _resourceManager.MainResourceMap;
        }

        public object? Convert(object? value, Type targetType, object? parameter, string language)
        {
            if (value is null)
                return null;

            return _resourceMap
                ?.TryGetValue($"{value.GetType().Name}_{value}")
                ?.ValueAsString
                ?? value.ToString();
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, string language)
            => throw new NotSupportedException();
    }
}
