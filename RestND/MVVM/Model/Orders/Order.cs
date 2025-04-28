using RestND.MVVM.Model.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.Orders
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

        #region Dish
        private Dish _Dish;

        public Dish Dish
        {
            get { return _Dish; }
            set { _Dish = value; }
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

        public Order( Employee AssignedEmployee, Dish dish, Table table)
        {
  
            assignedEmployee = AssignedEmployee;
            Dish = dish;
            Table = table;
            Table.Table_Status = false;
            _OrderCount++;
        }
        #endregion

        #region Default Constructor

        public Order()
        {
            _OrderCount++;
        }
        #endregion

  
    }
}
