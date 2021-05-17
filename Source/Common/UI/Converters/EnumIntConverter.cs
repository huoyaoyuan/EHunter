using System;
using Microsoft.UI.Xaml.Data;

namespace EHunter.Converters
{
    public sealed class EnumIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => System.Convert.ToInt32(value, null);
        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => Enum.ToObject(targetType, value);
    }
}
