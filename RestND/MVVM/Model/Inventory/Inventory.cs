using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class Inventory
    {
        #region Inventroy ID
 
        private string _Product_ID;

        public string Product_ID
        {
            get { return _Product_ID; }
            set { _Product_ID = value; }
        }

        #endregion

        #region Inventroy Name

        private string? _Product_Name;

        public string? Product_Name
        {
            get { return _Product_Name; }
            set { _Product_Name = value; }
        }
        #endregion

        #region Tolerance
        private double _Tolerance;

        public double Tolerance
        {
            get { return _Tolerance; }
            set
            {
                if (this.Tolerance >= 0)
                    _Tolerance = value;
            }
        }
        #endregion

        #region Quantity Available

        private int _Quantity_Available;

        public int Quantity_Available
        {
            get { return _Quantity_Available; }
            set { _Quantity_Available = value; }
        }

        #endregion

        #region Product Date
        private DateTime _Product_Date;
        public DateTime Product_Date
        {
            get { return _Product_Date; }
            set { _Product_Date = value; }
        }
        #endregion

        #region Constructor

        public Inventory(string productId, string productName, int quantityAvailable,double Tolerance)
        {
            this.Product_ID = productId;
            this.Product_Name = productName;
            this.Quantity_Available = quantityAvailable;
            this.Tolerance = Tolerance;
            this.Product_Date = DateTime.Now;
        }

        #endregion

        #region Default Constructor


        public Inventory()
        {
        }
        #endregion


    }
}
