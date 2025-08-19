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

            // Join roles so we can populate Employee_Role properly
            const string query = @"
                SELECT 
                    e.Employee_ID,
                    e.Employee_Name,
                    e.Employee_LastName,
                    e.Email,
                    e.Password,
                    e.Is_Active,
                    e.Role_ID,
                    r.Role_Name
                FROM employees e
                LEFT JOIN roles r ON e.Role_ID = r.Role_ID
                WHERE e.Is_Active = TRUE;";

            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                // Skip inactive (defense in depth; WHERE already filters)
                if (row.TryGetValue("Is_Active", out var isActive) && !Convert.ToBoolean(isActive))
                    continue;

                employees.Add(new Employee
                {
                    Employee_ID = Convert.ToInt32(row["Employee_ID"]),
                    Employee_Name = row["Employee_Name"]?.ToString(),
                    Employee_LastName = row["Employee_LastName"]?.ToString(),
                    Email = row["Email"]?.ToString(),
                    Password = row["Password"]?.ToString(), // you may want to omit returning hashed pwd
                    Is_Active = true,
                    Employee_Role = new Role
                    {
                        Role_ID = row["Role_ID"] is DBNull ? 0 : Convert.ToInt32(row["Role_ID"]),
                        Role_Name = row["Role_Name"]?.ToString()
                    }
                });
            }

            return employees;
        }
        #endregion

        #region Add Employee
        public override bool Add(Employee e)
        {
          

            // Hash password before insert
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(e.Password ?? string.Empty);

            // If Employee_ID is NOT auto-increment in your schema, keep it in the insert.
            // If it IS auto-increment, remove Employee_ID and @id parameter.
            const string query = @"
                INSERT INTO employees
                    (Employee_ID, Employee_Name, Employee_LastName, Role_ID, Password, Email, Is_Active)
                VALUES
                    (@id, @name, @last, @roleId, @password, @mail, TRUE);";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", e.Employee_ID),                    // remove if auto-increment
                new MySqlParameter("@name", (object?)e.Employee_Name ?? DBNull.Value),
                new MySqlParameter("@last", (object?)e.Employee_LastName ?? DBNull.Value),
                new MySqlParameter("@roleId", e.Employee_Role.Role_ID),         // <-- FK from object
                new MySqlParameter("@password", hashedPassword),
                new MySqlParameter("@mail", (object?)e.Email ?? DBNull.Value)
            ) > 0;
        }
        #endregion

        #region Update Employee
        public override bool Update(Employee e)
        {
          

           
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(e.Password ?? string.Empty);

            const string query = @"
                UPDATE employees
                   SET Employee_Name     = @name,
                       Employee_LastName = @last,
                       Role_ID           = @roleId,      
                       Password          = @password,
                       Email             = @mail
                 WHERE Employee_ID       = @id;";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", e.Employee_ID),
                new MySqlParameter("@name", (object?)e.Employee_Name ?? DBNull.Value),
                new MySqlParameter("@last", (object?)e.Employee_LastName ?? DBNull.Value),
                new MySqlParameter("@roleId", e.Employee_Role.Role_ID),    // <-- FK from object
                new MySqlParameter("@password", hashedPassword),
                new MySqlParameter("@mail", (object?)e.Email ?? DBNull.Value)
            ) > 0;
        }
        #endregion

        #region Delete Employee (soft delete)
        public override bool Delete(Employee d)
        {
            d.Is_Active = false;

            const string query = "UPDATE employees SET Is_Active = @active WHERE Employee_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@active", d.Is_Active),
                new MySqlParameter("@id", d.Employee_ID)) > 0;
        }
        #endregion
    }
}
