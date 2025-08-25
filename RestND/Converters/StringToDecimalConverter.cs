using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RestND.Converters
{
    public class StringToDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            return System.Convert.ToDecimal(value, culture).ToString(culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (value as string)?.Trim() ?? string.Empty;

            // Allow user to clear or be mid-typing: don't update source yet
            if (s.Length == 0)
                return Binding.DoNothing;

            // Accept both '.' and ',' as decimal separators
            var decSep = culture.NumberFormat.NumberDecimalSeparator;
            s = s.Replace(",", decSep).Replace(".", decSep);

            if (decimal.TryParse(s, NumberStyles.Number, culture, out var result))
                return result;

            // Invalid intermediate input -> keep old source value
            return Binding.DoNothing;
        }
    }
}