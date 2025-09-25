using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.VAT
{
    public class Vat
    {
        #region Vat ID

        private int _Vat_ID;

        public int Vat_ID
        {
            get { return _Vat_ID; }
            set { _Vat_ID = value; }
        }

        #endregion

        #region Vat Percentage
        private double _Percentage;
        public double Percentage
        {
            get { return _Percentage; }
            set { _Percentage = value; }
        }
        #endregion

        #region Constructor
        public Vat(double percentage)
        {
            this.Percentage = percentage;
        }
        #endregion

        #region Default Constructor
        public Vat()
        {

        }
        #endregion

        // we do not need is_active because the vat will only have an updating function.
        // it will present the current vat only.
    }
}
