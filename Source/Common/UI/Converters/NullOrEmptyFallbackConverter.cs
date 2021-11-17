using Microsoft.UI.Xaml.Data;

namespace EHunter.Converters
{
    public class NullOrEmptyFallbackConverter : IValueConverter
    {
        public object? FallbackValue { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, string language)
        {
            if (value is null || (value is string str && string.IsNullOrEmpty(str)))
                return FallbackValue;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }
}
