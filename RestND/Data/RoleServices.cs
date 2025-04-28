using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Data
{
    public class RoleServices : BaseService<Role>
    {
        #region Constructor
        public RoleServices() : base(new DatabaseOperations("127.0.0.1", "restnd", "root", "D123456N!")) { }
        #endregion

        #region Get All Roles
        public override List<Role> GetAll()
        {
            List<Role> roles = new List<Role>();
            string query = "SELECT * FROM Role";

            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                roles.Add(new Role
                {
                    Role_Name = Convert.ToString(row["Role_Name"]),
                    Role_Authorization = (AuthorizationStatus)Enum.Parse(typeof(AuthorizationStatus),
                        Convert.ToString(row["Role_Authorization"])),
                    Password = Convert.ToString(row["Password"]),
                    Email = Convert.ToString(row["Email"])
               
                });
            }

            return roles;
        }
        #endregion

        #region Add Role
        public override bool Add(Role r)
        {
            string query = "INSERT INTO role (Email, Password ,Role_Name , Role_Authorization) VALUES (@Email, @Password ,@Role_Name , @Role_Authorization)";

            return _db.ExecuteNonQuery(query,
                        new MySqlParameter("@Email", r.Email),
                        new MySqlParameter("@Password", r.Password),
                        new MySqlParameter("@Role_Name", r.Role_Name),
                        new MySqlParameter("@Role_Authorization", r.Role_Authorization)) > 0;

        }
        #endregion

        #region Update Role
        public override bool Update(Role r)
        {
            string query = "UPDATE role SET Email = @Email , Password = @Password , Role_Name = @Role_Name , Role_Authorization = @Role_Authorization WHERE Role_ID = @id";

            return _db.ExecuteNonQuery(query,
                        new MySqlParameter("@Email", r.Email),
                        new MySqlParameter("@Password", r.Password),
                        new MySqlParameter("@Role_Name", r.Role_Name),
                        new MySqlParameter("@Role_Authorization", r.Role_Authorization)) > 0;


        }
        #endregion

        #region Delete Role
        public override bool Delete(int roleId)
        {

            string query = "DELETE FROM role WHERE Role_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", roleId)) > 0;
        }
        #endregion
    }
}
