using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class Product
    {
        #region Product ID
 
        private int _Product_ID;

        public int Product_ID
        {
            get { return _Product_ID; }
            set { _Product_ID = value; }
        }

        #endregion

        #region Product Name

        private string? _Product_Name;

        public string? Product_Name
        {
            get { return _Product_Name; }
            set { _Product_Name = value; }
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

        #region Constructor

        public Product(int productId, string? productName, int quantityAvailable)
        {
            Product_ID = productId;
            Product_Name = productName;
            Quantity_Available = quantityAvailable;
        }

        #endregion

        #region Default Constructor

        public Product()
        {
            
        }

        #endregion



    }
}
