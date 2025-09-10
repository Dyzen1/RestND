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
                if (row.TryGetValue("Is_Active", out var isActive) && Convert.ToBoolean(isActive) == false)
                    continue; // Skip inactive orders
                orders.Add(new Order
                {
                    Order_ID = Convert.ToInt32(row["Order_ID"]),
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

        #region Add Final Order 
        public override bool Add(Order item)
        {
            return _transaction.AddOrder(item);
        }
        #endregion

        #region Add Starting Order(no dishes)

        public bool AddStartingOrder(Order o)
        {
            string query = "INSERT INTO orders (Employee_Name, Table_Number, Bill_Price, Is_Active) " +
                           "VALUES (@name, @number, @price, @active)";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", o.assignedEmployee.Employee_Name),
                new MySqlParameter("@number", o.Table.Table_Number),
                new MySqlParameter("@price", o.Bill.Price),
                new MySqlParameter("@active", o.Is_Active)
            ) > 0;
        }

        #endregion

        #region Delete Order
        public override bool Delete(Order d)
        {
            if(d.Order_ID <= 0)
                return false;
            if(d.DishInOrder.Count > 0)
            {
                d.Is_Active = false;
                string query = "UPDATE orders SET Is_Active = @active WHERE Order_ID = @id";
                return _db.ExecuteNonQuery(query,
                    new MySqlParameter("@active", d.Is_Active),
                    new MySqlParameter("@id", d.Order_ID)) > 0;
            }
            return _transaction.DeleteOrder(d.Order_ID);

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
