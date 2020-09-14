using System;
using Microsoft.UI.Xaml.Data;

namespace EHunter.Provider.Pixiv.Views
{
    internal class DateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => ((DateTime)value).ToLongDateString();

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }
}
