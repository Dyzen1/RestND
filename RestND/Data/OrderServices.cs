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
        #region Properties
        private readonly Transaction _transaction;
        #endregion

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

                    // Bill_Price is your column name:
                    Bill = new Bill
                    {
                        Price = Convert.ToDouble(row["Bill_Price"])
                    },

                    // NEW:
                    People_Count = Convert.ToInt32(row["People_Count"]),

                    Is_Active = Convert.ToBoolean(row["Is_Active"])
                });
            }

            return orders;
        }
        #endregion

        #region Add Final Order (via transaction)
        public override bool Add(Order item)
        {
            return _transaction.AddOrder(item);
        }
        #endregion

        #region Add Starting Order (no dishes) returns the new Order_ID
        public int AddStartingOrder(Order o)
        {
            const string insertQuery = @"
        INSERT INTO orders (Employee_ID, Table_Number, People_Count, Is_Active)
        VALUES (@Id, @number, @people, @active);";

            const string idQuery = "SELECT LAST_INSERT_ID();";

            _db.OpenConnection();
            using var tx = _db.Connection.BeginTransaction();

            try
            {
                // 1️. Insert the order (use same connection + transaction)
                _db.ExecuteNonQuery(insertQuery, _db.Connection, tx,
                    new MySqlParameter("@Id", o.assignedEmployee.Employee_ID),
                    new MySqlParameter("@number", o.Table.Table_Number),
                    new MySqlParameter("@people", o.People_Count),
                    new MySqlParameter("@active", o.Is_Active)
                );

                // 2️. Get the auto-incremented Order_ID safely (same connection + transaction)
                object result = _db.ExecuteScalar(idQuery, _db.Connection, tx);
                tx.Commit();
                return Convert.ToInt32(result);
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        #endregion

        #region Delete Order
        public override bool Delete(Order d)
        {
            if (d.Order_ID <= 0)
                return false;

            if (d.DishInOrder.Count > 0)
            {
                d.Is_Active = false;
                string softDelete = "UPDATE orders SET Is_Active = @active WHERE Order_ID = @id";
                return _db.ExecuteNonQuery(softDelete,
                    new MySqlParameter("@active", d.Is_Active),
                    new MySqlParameter("@id", d.Order_ID)) > 0;
            }

            return _transaction.DeleteOrder(d.Order_ID);
        }
        #endregion

        #region Update Order
        public override bool Update(Order o)
        {
            string query =
                "UPDATE orders " +
                "SET Employee_Name = @name, Table_Number = @number, People_Count = @people, Bill_Price = @price " +
                "WHERE Order_ID = @id"; 

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", o.Order_ID),
                new MySqlParameter("@name", o.assignedEmployee.Employee_Name),
                new MySqlParameter("@number", o.Table.Table_Number),
                new MySqlParameter("@people", o.People_Count),
                new MySqlParameter("@price", o.Bill.Price)
            ) > 0;
        }
        #endregion

        #region Deduct product quantityies
        public void DeductProductQuantities(int dishId)
        {
            string query = @"
                UPDATE inventory p
                JOIN products_in_dish pid ON p.Product_ID = pid.Product_ID
                SET p.Quantity_Available = p.Quantity_Available - pid.Amount_Usage
                WHERE pid.Dish_ID = @dishId;
            ";

            _db.ExecuteNonQuery(query, new MySqlParameter[]
            {
              new MySqlParameter("@dishId", dishId)
            });
        }
        #endregion

        #region Get Orders By Employee (Active + Inactive)
        public List<Order> GetOrdersByEmployeeId(int employeeId)
        {
            var orders = new List<Order>();

            const string query = @"
        SELECT 
            o.Order_ID,
            o.Employee_ID,
            o.Table_Number,
            o.People_Count,
            o.Total_Price,
            o.Is_Active,
            e.Employee_Name
        FROM orders o
        LEFT JOIN employees e ON e.Employee_ID = o.Employee_ID
        WHERE o.Employee_ID = @empId;
    ";

            var rows = _db.ExecuteReader(query, new MySqlParameter("@empId", employeeId));

            foreach (var row in rows)
            {
                var empName = row.TryGetValue("Employee_Name", out var n) ? (n?.ToString() ?? "") : "";

                orders.Add(new Order
                {
                    Order_ID = Convert.ToInt32(row["Order_ID"]),

                    assignedEmployee = new Employee
                    {
                        Employee_ID = Convert.ToInt32(row["Employee_ID"]),
                        Employee_Name = empName
                    },

                    Table = new Table
                    {
                        Table_Number = Convert.ToInt32(row["Table_Number"])
                    },

                    People_Count = Convert.ToInt32(row["People_Count"]),
                    Is_Active = Convert.ToBoolean(row["Is_Active"]),

                    // map Total_Price into your existing Bill.Price
                    Bill = new Bill
                    {
                        Price = row["Total_Price"] == DBNull.Value ? 0 : Convert.ToDouble(row["Total_Price"])
                    }
                });
            }

            return orders;
        }
        #endregion

    }
}
