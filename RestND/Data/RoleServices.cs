using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Employees;
using System;
using System.Collections.Generic;

namespace RestND.Data
{
    public class RoleServices() : BaseService<Role>(DatabaseOperations.Instance)
    {
        #region Get All Roles
        public override List<Role> GetAll()
        {
            var roles = new List<Role>();
            const string query = "SELECT Role_ID, Role_Name, Role_Authorization, Is_Active FROM roles WHERE Is_Active = TRUE";

            var rows = _db.ExecuteReader(query);
            foreach (var row in rows)
            {
                roles.Add(new Role
                {
                    Role_ID = Convert.ToInt32(row["Role_ID"]),
                    Role_Name = Convert.ToString(row["Role_Name"]),
                    Role_Authorization = (AuthorizationStatus)Convert.ToInt32(row["Role_Authorization"]),
                    Is_Active = Convert.ToBoolean(row["Is_Active"])
                });
            }

            return roles;
        }
        #endregion

        #region Add Role
        public override bool Add(Role r)
        {
            const string query = @"
INSERT INTO roles (Role_Name, Role_Authorization, Is_Active)
VALUES (@Role_Name, @Role_Authorization, @Is_Active);";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@Role_Name", r.Role_Name ?? (object)DBNull.Value),
                new MySqlParameter("@Role_Authorization", (int)r.Role_Authorization),
                new MySqlParameter("@Is_Active", r.Is_Active)
            ) > 0;
        }
        #endregion

        #region Update Role
        public override bool Update(Role r)
        {
            const string query = @"
UPDATE roles
SET Role_Name = @Role_Name,
    Role_Authorization = @Role_Authorization
WHERE Role_ID = @id;";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", r.Role_ID),
                new MySqlParameter("@Role_Name", r.Role_Name ?? (object)DBNull.Value),
                new MySqlParameter("@Role_Authorization", (int)r.Role_Authorization)
            ) > 0;
        }
        #endregion

        #region Delete Role (soft delete)
        public override bool Delete(Role r)
        {
            r.Is_Active = false;
            const string query = "UPDATE roles SET Is_Active = @active WHERE Role_ID = @id;";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@active", r.Is_Active),
                new MySqlParameter("@id", r.Role_ID)
            ) > 0;
        }
        #endregion
    }
}
