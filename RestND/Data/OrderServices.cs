using RestND.MVVM.Model.Employees;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mysqlx.Crud;
using MySql.Data.MySqlClient;
using System.Windows.Documents;

namespace RestND.Data
{
    public class OrderServices() : BaseService<Order>(DatabaseOperations.Instance)
    {
        #region Get employee name, table number and order date
        public override List<Order> GetAll()
        {
            var orders = new List<Order>();
            string query = "SELECT e.Employee_Name, t.Table_Number, o.Date FROM [Order] " +
                "JOIN Employee e ON o.Employee_ID = e.Employee_ID " +
                "JOIN [Table] t ON o.Table_ID = t.Table_ID;";
            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                orders.Add(new Order
                {
                    assignedEmployee = new Employee
                    {
                        Employee_Name = row["Employee_Name"].ToString(),
                    },
                    Table = new Table
                    {
                        Table_Number = Convert.ToInt32(row["Table_Number"])
                    }
                });

            }

            return orders;
        }
        #endregion

        #region Add Employee
        public override bool Add(Employee e)
        {
            string query = "INSERT INTO Employee (Employee_ID, Employee_Name, Employee_Role) VALUES (@id, @name, @role)";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", e.Employee_ID),
                new MySqlParameter("@role", e.Employee_Role),
                new MySqlParameter("@name", e.Employee_Name)) > 0;
        }
        #endregion

        #region Update Product
        public override bool Update(Employee e)
        {
            string query = "UPDATE Employee SET Employee_Name = @name, Employee_ID = @id, Employee_Role = @role WHERE Employee_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", e.Employee_Name),
                new MySqlParameter("@role", e.Employee_Role),
                new MySqlParameter("@id", e.Employee_ID)) > 0;
        }
        #endregion

        #region Delete Product
        public override bool Delete(int employeeID)
        {
            string query = "DELETE FROM Employee WHERE Employee_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", employeeID)) > 0;
        }
        #endregion
    }
}
