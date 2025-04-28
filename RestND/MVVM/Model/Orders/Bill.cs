using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.Orders
{
    public class Bill
    {

        #region Bill ID
        private int _Bill_ID;
        public int Bill_ID
        {
            get { return _Bill_ID; }
            set { _Bill_ID = value; }
        }
        #endregion

        #region Order Property

        private Order _Order;
        public Order Order
        {
            get { return _Order; }
            set { _Order = value; }
        }

        #endregion

        #region Price

        private double _Price;
        public double Price
        {
            get { return _Price; }
            set { _Price = value; }
        }

        #endregion

        #region Discount
        private Discount _Discount;
        public Discount Discount
        {
            get { return _Discount; }
            set { _Discount = value; }
        }
        #endregion

        #region constructor
        public Bill(Order order, double price)
        {
            Order = order;
            Price = price;
            
        }
        #endregion

        #region Constructor with discount
        public Bill(Order order, double price,Discount discount)
        {
            discount = discount;
            Order = order;
            Price = price;

        }
        #endregion

        #region Default Constructor
        public Bill()
        {

        }
        #endregion



    }
}
