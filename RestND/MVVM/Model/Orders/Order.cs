using RestND.MVVM.Model.Employees;
using RestND.MVVM.Model.Tables;
using System.Collections.Generic;


namespace RestND.MVVM.Model.Orders
{
    public class Order
    {
        #region Order ID
        private int _Order_ID;
        public int Order_ID
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

        #region dishes in order
        private List<DishInOrder> _DishInOrder = new();

        public List<DishInOrder> DishInOrder 
        {
            get { return _DishInOrder; }
            set { _DishInOrder = value; }
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

        #region Order Bill
        private Bill _Bill;
        public Bill Bill
        {
            get { return _Bill; }
            set { _Bill = value; }
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
        public Order( Employee AssignedEmployee,  Table table)
        {
            this.Bill = new Bill();
            this.assignedEmployee = AssignedEmployee;
            this.Table = table;
            this.DishInOrder = null;
            this.Is_Active = true;
            _OrderCount++;
        }
        #endregion

        #region Default constructor
        public Order()
        {
            _OrderCount++;
        }
        #endregion
    }
}
