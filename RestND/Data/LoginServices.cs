using MySql.Data.MySqlClient;
using RestND.MVVM.Model.Employees;
using System;

namespace RestND.Data
{
    public class LoginServices
    {
        private readonly DatabaseOperations _db = DatabaseOperations.Instance;

        #region Verify Login
        public Employee? AuthenticateById(int employeeId, string plainPassword)
        {
            const string sql = @"
                        SELECT 
                            e.Employee_ID,
                            e.Employee_Name,
                            e.Employee_Lastname,
                            e.Password,
                            e.Role_ID,
                            e.Is_Active AS EmpActive,
                            r.Role_Name,
                            r.Permissions,
                            r.Is_Active AS RoleActive
                        FROM employees e
                        LEFT JOIN roles r ON e.Role_ID = r.Role_ID
                        WHERE e.Employee_ID = @id
                        LIMIT 1;";

            var rows = _db.ExecuteReader(sql, new MySqlParameter("@id", employeeId));
            if (rows.Count == 0) return null;

            var row = rows[0];

            var storedHash = Convert.ToString(row["Password"]);
            if (string.IsNullOrEmpty(storedHash)) return null;

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(plainPassword, storedHash))
                return null;

            // Optional: you can block inactive accounts/roles here
            var empActive = Convert.ToBoolean(row["EmpActive"]);
            if (!empActive) return null;

            Role? role = null;
            if (row["Role_ID"] != DBNull.Value)
            {
                role = new Role
                {
                    Role_ID = Convert.ToInt32(row["Role_ID"]),
                    Role_Name = Convert.ToString(row["Role_Name"]),
                    Permissions = row["Permissions"] == DBNull.Value
                        ? AppPermission.None
                        : (AppPermission)Convert.ToInt32(row["Permissions"]),
                    Is_Active = row["RoleActive"] != DBNull.Value && Convert.ToBoolean(row["RoleActive"])
                };
            }

            // Build employee
            var emp = new Employee
            {
                Employee_ID = Convert.ToInt32(row["Employee_ID"]),
                Employee_Name = Convert.ToString(row["Employee_Name"]),
                Employee_LastName = Convert.ToString(row["Employee_Lastname"]),
                Password = storedHash, // keep hash if your model has it
                Employee_Role = role,
                Is_Active = empActive
            };

            return emp;
        }
        #endregion
    }
}
