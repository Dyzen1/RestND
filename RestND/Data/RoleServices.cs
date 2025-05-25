using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Employees;
using RestND.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RestND.Data
{
    public class RoleServices() : BaseService<Role>(DatabaseOperations.Instance)
    {
        #region Get All Roles
        public override List<Role> GetAll()
        {
            List<Role> roles = new List<Role>();
            var query = "SELECT * FROM roles";

            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                roles.Add(new Role
                {
                    Role_Name = Convert.ToString(row["Role_Name"]),
                    Role_Authorization = (AuthorizationStatus)Enum.Parse(typeof(AuthorizationStatus),
                        Convert.ToString(row["Role_Authorization"]))
                });
            }

            return roles;
        }
        #endregion

        #region Add Role
        public override bool Add(Role r)
        {
            string query = "INSERT INTO roles (Email, Password ,Role_Name , Role_Authorization) VALUES (@Email, @Password ,@Role_Name , @Role_Authorization)";

            return _db.ExecuteNonQuery(query,
                        new MySqlParameter("@Role_Name", r.Role_Name),
                        new MySqlParameter("@Role_Authorization", r.Role_Authorization)) > 0;

        }
        #endregion

        #region Update Role
        public override bool Update(Role r)
        {
            string query = "UPDATE roles SET Email = @Email , Password = @Password , Role_Name = @Role_Name , Role_Authorization = @Role_Authorization WHERE Role_ID = @id";

            return _db.ExecuteNonQuery(query, 
                        new MySqlParameter("@id", r.Role_ID),
                        new MySqlParameter("@Role_Name", r.Role_Name),
                        new MySqlParameter("@Role_Authorization", r.Role_Authorization)) > 0;


        }
        #endregion

        #region Delete Role
        public override bool Delete(int roleId)
        {
            if (roleId <= 0) return false;

            string query = "DELETE FROM roles WHERE Role_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", roleId)) > 0;
        }
        #endregion
    }
}
