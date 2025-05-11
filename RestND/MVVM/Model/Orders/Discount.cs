using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.Orders
{
    public class Discount
    {
        #region Discount ID

        private string _Discount_ID;

        public string Discount_ID
        {
            get { return _Discount_ID; }
            set { _Discount_ID = value; }
        }

        #endregion

        #region Discount Name

        private string? _Discount_Name;

        public string? Discount_Name
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
            this.Discount_Name = name;
            this.Discount_Percentage = discountPercentage;

        }
        #endregion

        #region Default Constructor


        public Discount()
        {

        }
        #endregion



    }
}
