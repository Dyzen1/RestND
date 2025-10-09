using System;
using System.Globalization;
using System.Windows.Data;

namespace RestND.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
                return isActive ? 1.0 : 0.3;  // 1.0 = fully visible, 0.3 = dimmed
            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
