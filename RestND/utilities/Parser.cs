using System;

namespace RestND.utilities
{
    public class Parser
    {
        public static double ParseToDouble(object value, double fallback = 0)
        {
            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return fallback;
            }
        }

        public static int ParseToInt(object value, int fallback = 0)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return fallback;
            }
        }

        public static bool ParseToBool(object value, bool fallback = false)
        {
            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return fallback;
            }
        }
    }
}
