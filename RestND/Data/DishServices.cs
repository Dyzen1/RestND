using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestND.Data
{
    public class DishService : BaseService<Dish>
    {
        public DishService() : base(new DatabaseOperations("127.0.0.1", "restnd", "root", "D123456N!")) { }

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

        public override bool Add(Dish d)
        {
            string query = "INSERT INTO dish (Dish_Name, Dish_Price, Allergen_Notes, Availability_Status, Dish_Type) " +
                           "VALUES (@name, @price, @notes, @status, @type)";

            bool dishAdded = _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", d.Dish_Name),
                new MySqlParameter("@price", d.Dish_Price),
                new MySqlParameter("@notes", d.Allergen_Notes),
                new MySqlParameter("@status", d.Availability_Status),
                new MySqlParameter("@type", d.Dish_Type.DishType_Name) // Save DishType 
            ) > 0;

            if (!dishAdded)
                return false;

            int newDishId = Convert.ToInt32(_db.ExecuteReader("SELECT LAST_INSERT_ID();")[0].Values.First());

            if (d.ProductUsage != null && d.ProductUsage.Count > 0)
            {
                var productInDishService = new ProductInDishService();
                bool productsAdded = productInDishService.AddProductsToDish(newDishId, d.ProductUsage);

                return productsAdded;
            }

            return true;
        }

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

        public override bool Delete(int dishId)
        {
            string query = "DELETE FROM dish WHERE Dish_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", dishId)) > 0;
        }
    }
}
