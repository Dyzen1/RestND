using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.utilities
{
    public class Parser
    {

      public static double ParseToDouble(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return Convert.ToDouble(value);
        }
        public static int ParseToInt(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Convert.ToInt32(value);
        }

        public static bool ParseToBool(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null");

            return Convert.ToBoolean(value);
        }




    }
}
