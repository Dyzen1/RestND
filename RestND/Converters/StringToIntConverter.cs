using RestND.utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RestND.Converters
{
    // helper to bind int properties to TextBoxes
    public class StringToIntConverter : IValueConverter

    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Model to View: convert int to string
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // View to Model: safely convert string to int using Parser
            return Parser.ParseToInt(value, 0);
        }
    }

}
