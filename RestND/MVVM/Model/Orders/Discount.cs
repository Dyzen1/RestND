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

        private int _Discount_ID;

        public int Discount_ID
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

        #region Is_Active - a property for knowing wheather the discount has been deleted or not
        private bool _Is_Active;
        public bool Is_Active
        {
            get { return _Is_Active; }
            set { _Is_Active = value; }
        }
        #endregion

        #region Constructor
        public Discount(string name , double discountPercentage)
        {
            this.Discount_Name = name;
            this.Discount_Percentage = discountPercentage;
            this.Is_Active = true; 
        }
        #endregion

        #region Default Constructor
        public Discount()
        {

        }
        #endregion
    }
}
