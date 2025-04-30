using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestND.Data
{
    public class DishService : BaseService<Dish>
    {
        private readonly Transaction _transaction = new Transaction(_db);

        #region Constructor
        public DishService() : base(new DatabaseOperations("127.0.0.1", "restnd", "root", "D123456N!")) { }
        #endregion

        #region Get All Dishes
        public override List<Dish> GetAll()
        {
            var dishes = new List<Dish>();
            string query = "SELECT * FROM dish";
            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                dishes.Add(new Dish
                {
                    Dish_ID = Convert.ToInt32(row["Dish_ID"]),
                    Dish_Name = row["Dish_Name"].ToString(),
                    Dish_Price = Convert.ToInt32(row["Dish_Price"]),
                    Allergen_Notes = row["Allergen_Notes"].ToString(),
                    Availability_Status = Convert.ToBoolean(row["Availability_Status"]),
                    Dish_Type = new DishType
                    {
                        DishType_Name = row["Dish_Type"].ToString()
                    }
                });
            }

            return dishes;
        }
        #endregion

        #region Add Dish
        public override bool Add(Dish d)
        {
            return _transaction.Add(d);
        }
        #endregion

        #region Delete Dish
        public override bool Delete(int dishId)
        { 
          return _transaction.Delete(dishId);

        }
        #endregion

        #region Update Dish
        public override bool Update(Dish d)
        {
            string query = "UPDATE dish SET Dish_Name = @name, Dish_Price = @price, Allergen_Notes = @notes, " +
                           "Availability_Status = @status, Dish_Type = @type WHERE Dish_ID = @id";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", d.Dish_Name),
                new MySqlParameter("@price", d.Dish_Price),
                new MySqlParameter("@notes", d.Allergen_Notes),
                new MySqlParameter("@status", d.Availability_Status),
                new MySqlParameter("@type", d.Dish_Type.DishType_Name), 
                new MySqlParameter("@id", d.Dish_ID)
            ) > 0;
        }
        #endregion


    }
}
