using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class Order
    {
        #region Order ID

        private int _Order_ID;

        public int Order_ID
        {
            get { return _Order_ID; }
            set
            {
                _Order_ID = value;
            }
        }

        #endregion

        #region Employee

        

       
        private Employee _AssignedEmployee;

        public Employee assignedEmployee
        {
            get { return _AssignedEmployee; }
            set { _AssignedEmployee = value; }
        }
        #endregion

        #region Dish Amount

        

        
        private int _Dish_Amount;

        public int Dish_Amount
        {
            get { return _Dish_Amount; }
            set { _Dish_Amount =value; }
        }
        #endregion

        #region Order Date

        private DateTime _Order_Date;

        public DateTime Order_Date
        {
            get { return _Order_Date; }
            set { _Order_Date = value; }
        }

        #endregion

        #region Dish
        private Dish _Dish;

        public Dish Dish
        {
            get { return _Dish; }
            set { _Dish = value; }
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

        public Order(int orderId, Employee assignedEmployee, Dish dish, int dishAmount)
        {
            Order_ID = orderId;
            _AssignedEmployee = assignedEmployee;
            Dish = dish;
            Dish_Amount = dishAmount;
            Order_Date = DateTime.Now;
        }
        #endregion

        #region Default Constructor

        

   
        public Order()
        {
            
        }
    #endregion
    }
}
