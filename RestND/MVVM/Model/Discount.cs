using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class Discount
    {
        #region Discount Name

        private string _Discount_Name;

        public string Discount_Name
        {
            get { return _Discount_Name; }
            set { _Discount_Name = value; }
        }

        #endregion


        #region Discount Percentage
        private double _Discount_Percentage;
        public double Discount_Percentage
        {
            get { return _Discount_Percentage; }
            set { _Discount_Percentage = value; }
        }
        #endregion

        #region constructor

        public Discount(string name , double discountPercentage)
        {
            Discount_Name = name;
            Discount_Percentage = discountPercentage;

        }
        #endregion

        #region Default Constructor

        public Discount()
        {
            
        }

        #endregion


        #region Equals
        public override bool Equals(object obj)
        {
            if (obj is Discount other)
                return _Discount_Name == other._Discount_Name;
            return false;
        }
        #endregion

    }
}
