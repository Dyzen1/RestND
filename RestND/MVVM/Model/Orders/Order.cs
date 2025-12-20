using CommunityToolkit.Mvvm.ComponentModel;
using RestND.MVVM.Model.Employees;
using RestND.MVVM.Model.Tables;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace RestND.MVVM.Model.Orders
{
    public partial class Order : ObservableObject
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

        #region Dishes in order (Observable)
        [ObservableProperty]
        private ObservableCollection<DishInOrder> dishInOrder = new();
        #endregion

        #region Table 
        private Table _Table;
        public Table Table
        {
            get { return _Table; }
            set { _Table = value; }
        }
        #endregion

        #region People_Count (diners)
        private int _People_Count;
        public int People_Count
        {
            get { return _People_Count; }
            set { _People_Count = value; }
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

        #region Computed Status Text
        public string StatusText => Is_Active ? "In Progress" : "Finished";
        #endregion

        #region Constructor
        public Order( Employee AssignedEmployee,  Table table)
        {
            this.Bill = new Bill();
            this.assignedEmployee = AssignedEmployee;
            this.Table = table;
            this.DishInOrder = new ObservableCollection<DishInOrder>(); // ✅ not null
            this.Is_Active = true;
            this.People_Count = 1;
        }
        #endregion

        #region Default constructor
        public Order()
        {
            this.Is_Active = true;
            this.People_Count = 1;
        }
        #endregion
    }
}
