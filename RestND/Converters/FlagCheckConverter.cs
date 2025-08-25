using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace RestND.Converters
{
    public class FlagCheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;
            long v = System.Convert.ToInt64(value);
            long p = System.Convert.ToInt64(parameter);
            return (v & p) == p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) return value;
            bool isChecked = value is bool b && b;
            long p = System.Convert.ToInt64(parameter);



            return Binding.DoNothing;
        }
    }
}
