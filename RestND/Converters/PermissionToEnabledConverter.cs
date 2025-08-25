using System;
using System.Globalization;
using System.Windows.Data;
using RestND.MVVM.Model.Employees; // AppPermission
using RestND.MVVM.Model.Security;  // AuthContext

namespace RestND.Converters
{
    /// <summary>
    /// Returns true if the current user has the required AppPermission.
    /// Supports:
    ///   1) Per-resource: <conv:PermissionToEnabledConverter x:Key="TablesPerm" Required="Tables"/>
    ///   2) Shared + parameter: IsEnabled="{Binding AuthVersion, Converter={StaticResource PermToEnabled}, ConverterParameter=Tables}"
    /// </summary>
    public class PermissionToEnabledConverter : IValueConverter
    {
        /// <summary>
        /// Optional: set in XAML as enum name ("Tables") or number.
        /// </summary>
        public string? Required { get; set; } // keep string so XAML can pass names

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Priority: ConverterParameter -> Required property
            if (TryGetPermission(parameter, out var fromParam))
                return AuthContext.Has(fromParam);

            if (TryGetPermission(Required, out var fromProp))
                return AuthContext.Has(fromProp);

            return false; // nothing specified -> lock down
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;

        private static bool TryGetPermission(object? source, out AppPermission perm)
        {
            perm = AppPermission.None;
            if (source == null) return false;

            // strings: enum name or numeric value
            if (source is string s)
            {
                if (int.TryParse(s, out var num))
                {
                    perm = (AppPermission)num;
                    return true;
                }
                if (Enum.TryParse<AppPermission>(s, true, out var parsed))
                {
                    perm = parsed;
                    return true;
                }
                return false;
            }

            // numeric types (rare via XAML)
            try
            {
                perm = (AppPermission)System.Convert.ToInt64(source);
                return true;
            }
            catch { return false; }
        }
    }
}
