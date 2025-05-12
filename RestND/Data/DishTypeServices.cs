using RestND.MVVM.Model.Orders;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RestND.Data
{
    public class DishTypeServices() : BaseService<DishType>(DatabaseOperations.Instance)
    {
        #region Get All Dish Types

        public override List<DishType> GetAll()
        {
            var types = new List<DishType>();
            var query = "SELECT * FROM dish_types";
            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                types.Add(new DishType
                {
                    DishType_ID = row["DishType_ID"].ToString(),
                    DishType_Name = row["DishType_Name"].ToString()
                });
            }

            return types;
        }
        #endregion

        #region Add Dish Type
        public override bool Add(DishType d)
        {
            string query = "INSERT INTO dish_types (DishType_ID, DishType_Name) VALUES (@id, @name)";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", d.DishType_ID),
                new MySqlParameter("@name", d.DishType_Name)) > 0;

        }
        #endregion

        #region Update DishType
        public override bool Update(DishType d)
        {
            string query = "UPDATE dish_types SET DishType_Name = @name, DishType_ID = @id WHERE DishType_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", d.DishType_Name),
                new MySqlParameter("@id", d.DishType_ID)) > 0;

        }
        #endregion

        #region Delete DishType
        public override bool Delete(string DishType_ID)
        {
            string query = "DELETE FROM dish_type WHERE DishType_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", DishType_ID)) > 0;
        }
        #endregion
    }
}
