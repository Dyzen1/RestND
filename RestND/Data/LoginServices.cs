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

        public int GetPassword(int id)
        {
            var query = "SELECT Password FROM employees WHERE ID = @id";
            int result = Convert.ToInt32(_db.ExecuteReader(query, new MySqlParameter("@id", id)));
            
            return result;
        }

        #endregion
    }
}
