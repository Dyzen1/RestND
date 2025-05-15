using System;

namespace RestND.MVVM.Model.Orders
{
    public class Bill
    {
        #region Bill ID
        private string _Bill_ID;
        public string Bill_ID
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

        #region Bill Date
        private DateTime _Bill_Date;
        public DateTime Bill_Date
        {
            get { return _Bill_Date; }
            set { _Bill_Date = value; }
        }
        #endregion

        #region Constructor without Discount

        public Bill(Order order,double price)
        {
            this.Order = order;
            this.Price = price;
            this.Bill_Date = DateTime.Now;
        }


        #endregion

        #region Constructor with Discount
        public Bill(Order order, Discount discount,double price)
        {
            this.Order = order;
            this.Price = price;
            this.Discount = discount;
            this.Bill_Date = DateTime.Now;
        }
        #endregion


        #region Default constructor
        public Bill() { this.Price = 0; }
        #endregion

    }
}