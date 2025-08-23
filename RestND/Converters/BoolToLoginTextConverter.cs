using System;
using System.Globalization;
using System.Windows.Data;

namespace RestND.Converters
{
    public class BoolToLoginTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Logout" : "Login";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
