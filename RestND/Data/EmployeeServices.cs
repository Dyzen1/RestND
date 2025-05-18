using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Employees;
using System;
using System.Collections.Generic;

namespace RestND.Data
{
    public class EmployeeServices() : BaseService<Employee>(DatabaseOperations.Instance)
    {
        #region Get All Employees
        public override List<Employee> GetAll()
        {
            var employees = new List<Employee>();
            string query = "SELECT * FROM employees";
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
                    },
                    Password = Convert.ToString(row["Password"]),
                    Email = Convert.ToString(row["Email"])
                });
            }

            return employees;
        }
        #endregion

        #region Add Employee
        public override bool Add(Employee e)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(e.Password);

            string query = "INSERT INTO employees (Employee_ID, Employee_Name, Employee_Role, Password, Email) VALUES (@id, @name, @role, @password, @mail)";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", e.Employee_ID),
                new MySqlParameter("@name", e.Employee_Name),
                new MySqlParameter("@role", e.Employee_Role?.Role_Name),
                new MySqlParameter("@password", hashedPassword),
                new MySqlParameter("@mail", e.Email)
            ) > 0;
        }

        #endregion

        #region Update Employee
        public override bool Update(Employee e)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(e.Password);

            string query = "UPDATE employees SET Employee_Name = @name, Employee_Role = @role, Password = @password, Email = @mail WHERE Employee_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", e.Employee_ID),
                new MySqlParameter("@name", e.Employee_Name),
                new MySqlParameter("@role", e.Employee_Role?.Role_Name),
                new MySqlParameter("@password", hashedPassword),
                new MySqlParameter("@mail", e.Email)
            ) > 0;
        }

        #endregion

        #region Delete Employee
        public override bool Delete(int  employeeID)
        {
            string query = "DELETE FROM employees WHERE Employee_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", employeeID)) > 0;
        }
        #endregion
    }
}
