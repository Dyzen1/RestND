using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class Order
    {

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

        #region Order count
        private static int _OrderCount = 0;

        public static int OrderCount
        {
            get { return _OrderCount; }
        }
        #endregion

        #region Table 
        private Table _Table;
        public Table Table
        {
            get { return _Table; }
            set { _Table = value; }
        }
        #endregion

        #region constructor

        public Order( Employee AssignedEmployee, Dish dish, int dishAmount,Table table)
        {
  
            assignedEmployee = AssignedEmployee;
            Dish = dish;
            Dish_Amount = dishAmount;
            Table = table;
            Table.Table_Status = false;
            _OrderCount++;
        }
        #endregion

        #region Constructor that takes discount

        public Order(Employee AssignedEmployee, Dish dish, int dishAmount,Discount discountType, Table table)
        {
             _OrderCount++;
            assignedEmployee = AssignedEmployee;
            Dish = dish;
            Dish_Amount = dishAmount;
            Discount = discountType;
            Table = table;
            Table.Table_Status = false;
        }
        #endregion

        #region Default Constructor




        public Order()
        {
            
        }
    #endregion
    }
}
