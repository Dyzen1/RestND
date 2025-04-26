using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestND.Data
{
    // Service that manages dishes: loading, adding, updating, deleting
    public class DishService : BaseService<Dish>
    {
        public DishService() : base(new DatabaseOperations("127.0.0.1", "restnd", "root", "D123456N!")) { }

        // Loads all dishes from the 'dish' table
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
                    Dish_Type = Enum.Parse<DishType>(row["Dish_Type"].ToString()) // Parsing the Dish Type from string to enum
                });
            }

            return dishes;
        }

        // Adds a new dish and its associated products into the database
        public override bool Add(Dish d)
        {
            string query = "INSERT INTO dish (Dish_Name, Dish_Price, Allergen_Notes, Availability_Status, Dish_Type) " +
                           "VALUES (@name, @price, @notes, @status, @type)";

            bool dishAdded = _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", d.Dish_Name),
                new MySqlParameter("@price", d.Dish_Price),
                new MySqlParameter("@notes", d.Allergen_Notes),
                new MySqlParameter("@status", d.Availability_Status),
                new MySqlParameter("@type", d.Dish_Type.ToString())
            ) > 0;

            if (!dishAdded)
                return false;

            // Get the new dish ID after insertion
            int newDishId = Convert.ToInt32(_db.ExecuteReader("SELECT LAST_INSERT_ID();")[0].Values.First());

            // Insert associated products if any
            if (d.ProductUsage != null && d.ProductUsage.Count > 0)
            {
                var productInDishService = new ProductInDishService();
                bool productsAdded = productInDishService.AddProductsToDish(newDishId, d.ProductUsage);

                return productsAdded;
            }

            return true;
        }

        // Updates an existing dish
        public override bool Update(Dish d)
        {
            string query = "UPDATE dish SET Dish_Name = @name, Dish_Price = @price, Allergen_Notes = @notes, " +
                           "Availability_Status = @status, Dish_Type = @type WHERE Dish_ID = @id";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", d.Dish_Name),
                new MySqlParameter("@price", d.Dish_Price),
                new MySqlParameter("@notes", d.Allergen_Notes),
                new MySqlParameter("@status", d.Availability_Status),
                new MySqlParameter("@type", d.Dish_Type.ToString()),
                new MySqlParameter("@id", d.Dish_ID)
            ) > 0;
        }

        // Deletes a dish by its ID
        public override bool Delete(int dishId)
        {
            string query = "DELETE FROM dish WHERE Dish_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", dishId)) > 0;
        }
    }
}
