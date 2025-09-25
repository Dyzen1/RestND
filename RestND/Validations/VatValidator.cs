using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Validations
{
    public class VatValidator : GeneralValidations
    {
        public bool ValidRate(string rate, out double parsed, out string err)
        {
            err = string.Empty;
            parsed = 0;

            if (!double.TryParse(rate, out parsed))
            {
                err = "VAT rate must be a valid number.";
                return false;
            }

            if (!CheckPosNumDouble(parsed, out var posErr))
            {
                err = posErr;
                return false;
            }

            return true;
        }
    }
}
