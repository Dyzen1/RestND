using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Data
{
    public class EmployeeServices : BaseService<Employee>
    {
        #region Constructor
        public EmployeeServices() : base(new DatabaseOperations("127.0.0.1", "restnd", "root", "D123456N!")) { }
        #endregion

        #region Get All Employees
        public override List<Employee> GetAll()
        {
            var employees = new List<Employee>();
            string query = "SELECT * FROM Employee";
            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                employees.Add(new Employee
                {
                    Employee_ID = Convert.ToInt32(row["Employee_ID"]),
                    Employee_Name = row["Employee_Name"].ToString(),
                    Employee_Role = new Role
                    {
                        Role_Name = row["Role_Name"].ToString()
                    }
                });
            }

            return employees;
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
