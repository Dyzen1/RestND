using System;
using System.Globalization;
using System.Windows.Data;

namespace RestND.Converters
{
    // Bool -> Opacity
    // Default: true -> 1.0, false -> 0.3   (perfect for Dish.In_Stock)
    // If ConverterParameter="invert": true -> 0.3, false -> 1.0  (perfect for Table_Status meaning occupied)
    public class BoolToOpacityConverter : IValueConverter
    {
        public double TrueOpacity { get; set; } = 1.0;
        public double FalseOpacity { get; set; } = 0.3;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool flag)
                return 1.0;

            bool invert = string.Equals(parameter?.ToString(), "invert", StringComparison.OrdinalIgnoreCase);

            if (!invert)
                return flag ? TrueOpacity : FalseOpacity;   // true=1, false=0.3

            return flag ? FalseOpacity : TrueOpacity;       // inverted
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
