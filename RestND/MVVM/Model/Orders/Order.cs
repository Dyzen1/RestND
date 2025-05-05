using RestND.MVVM.Model.Employees;
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
        #region Order ID
        private string _Order_ID;
        public string Order_ID
        {
            get { return _Order_ID; }
            set { _Order_ID = value; }
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

        #region Amount of dish in order
        private AmountOfDIshesInOrder _AmountOfDIshesInOrder;

        public AmountOfDIshesInOrder AmountOfDIshesInOrder
        {
            get { return _AmountOfDIshesInOrder; }
            set { _AmountOfDIshesInOrder = value; }
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

        #region Constructor
        public Order( Employee AssignedEmployee,  Table table,AmountOfDIshesInOrder amount)
        {
            assignedEmployee = AssignedEmployee;
            Table = table;
            Table.Table_Status = false;
            AmountOfDIshesInOrder = amount;
            _OrderCount++;
        }
        #endregion


    }
}
