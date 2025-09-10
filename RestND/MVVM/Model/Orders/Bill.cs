using System;

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

        #region Is_Active - a property for knowing wheather the bill has been deleted or not
        private bool _Is_Active;
        public bool Is_Active
        {
            get { return _Is_Active; }
            set { _Is_Active = value; }
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

        #region IsPaid
        private bool _Is_Paid;
        public bool Is_Paid
        {
            get { return _Is_Paid; }
            set { _Is_Paid = value; }
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
            this.Is_Paid = false;
            this.Is_Active = true;
            this.Discount = null;
        }


        #endregion

        #region Default constructor
        public Bill() { this.Price = 0; }
        #endregion

    }
}