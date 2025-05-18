using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Data
{
    public class LoginServices 
    {
        private readonly DatabaseOperations _db = DatabaseOperations.Instance;

        #region Get Password based on user's id

        public string? GetPassword(int employeeId)
        {
            string query = "SELECT Password FROM employees WHERE Employee_ID = @id";
            var rows = _db.ExecuteReader(query, new MySqlParameter("@id", employeeId));

            if (rows.Count == 0) return null;

            return rows[0]["Password"]?.ToString();
        }


        #endregion

        #region check stored password 

        public bool ValidateLogin(int employeeId, string plainPassword)
        {
            var storedHash = GetPassword(employeeId);
            if (string.IsNullOrEmpty(storedHash)) return false;

            return BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);
        }

        #endregion
    }
}
