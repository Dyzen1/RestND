
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
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
                if (row.TryGetValue("Is_Active", out var isActive) && Convert.ToBoolean(isActive) == false)
                    continue; // Skip inactive dishtypes
                types.Add(new DishType
                {
                    DishType_ID = Convert.ToInt32(row["DishType_ID"]),
                    DishType_Name = row["DishType_Name"].ToString()
                });
            }

            return types;
        }
        #endregion

        #region Add Dish Type
        public override bool Add(DishType d)
        {
            string query = "INSERT INTO dish_types (DishType_Name) VALUES (@name)";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", d.DishType_Name)) > 0;

        }
        #endregion

        #region Update DishType
        public override bool Update(DishType d)
        {
            string query = "UPDATE dish_types SET DishType_Name = @name WHERE DishType_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", d.DishType_Name)) > 0;

        }
        #endregion

        #region Delete DishType (not really deleting, just marking as inactive)
        public override bool Delete(DishType d)
        {
            d.Is_Active = false;
            string query = "UPDATE dish_types SET Is_Active = @active WHERE DishType_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@active", d.Is_Active),
                new MySqlParameter("@id", d.DishType_ID)) > 0;
        }

        #endregion
    }
}
