using RestND.MVVM.Model.Employees;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using RestND.MVVM.Model.Orders;
using RestND.MVVM.Model.Tables;

namespace RestND.Data
{
    public class OrderServices : BaseService<Order>
    {
        private readonly Transaction _transaction;

        #region Constructor
        public OrderServices() : base(DatabaseOperations.Instance)
        {
            _transaction = new Transaction(_db);
        }
        #endregion

        #region Get All Orders
        public override List<Order> GetAll()
        {
            var orders = new List<Order>();
            string query = "SELECT * FROM orders";
            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                orders.Add(new Order
                {
                    Order_ID = row["Order_ID"].ToString(),
                    assignedEmployee = new Employee
                    {
                        Employee_Name = row["Employee_Name"].ToString()
                    },
                    Table = new Table
                    {
                        Table_Number = Convert.ToInt32(row["Table_Number"])
                    },
                    Bill = new Bill
                    {
                        Price = Convert.ToDouble(row["Price"] )
                    }
                });
            }

            return orders;
        }
        #endregion

        #region Add Order
        public override bool Add(Order item)
        {
            return _transaction.AddOrder(item);
        }
        #endregion

        #region Delete Order
        public override bool Delete(string orderID)
        {
            return _transaction.DeleteOrder(orderID);
        }
        #endregion

        #region Update Order
        public override bool Update(Order o)
        {
            string query = "UPDATE orders SET Order_ID = @id, Employee_Name = @name, Table_Number = @number, Bill_Price = @price" +
                           "WHERE Dish_ID = @id";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", o.Order_ID),
                new MySqlParameter("@name", o.assignedEmployee.Employee_Name),
                new MySqlParameter("@number", o.Table.Table_Number),
                new MySqlParameter("@price", o.Bill.Price)
            ) > 0;
        }
        #endregion
    }
}
