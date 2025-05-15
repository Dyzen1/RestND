using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using RestND.MVVM.Model.Employees;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestND.Data
{
    public class DishServices : BaseService<Dish>
    {
        private readonly Transaction _transaction;

        #region Constructor
        public DishServices() : base(DatabaseOperations.Instance)
        {
            _transaction = new Transaction(_db); 
        }
        #endregion

        #region Get All Dishes
        public override List<Dish> GetAll()
        {
            var dishes = new List<Dish>();
            string query = "SELECT * FROM dishes";
            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                dishes.Add(new Dish
                {
                    Dish_ID = row["Dish_ID"].ToString(),
                    Dish_Name = row["Dish_Name"].ToString(),
                    Dish_Price = Convert.ToInt32(row["Dish_Price"]),
                    Allergen_Notes = row["Allergen_Notes"].ToString()
                        .Split(',')
                        .Select(note => (AllergenNotes)Enum.Parse(typeof(AllergenNotes), note.Trim()))
                        .ToList(),
                    Availability_Status = Convert.ToBoolean(row["Availability_Status"]),
                    Tolerance = Convert.ToDouble(row["Tolerance"]),
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
            return _transaction.AddDish(d);
        }
        #endregion

        #region Delete Dish
        public override bool Delete(string dishId)
        { 
          return _transaction.DeleteDish(dishId);

        }
        #endregion

        #region Update Dish
        public override bool Update(Dish d)
        {
            string query = "UPDATE dishes SET Dish_Name = @name, Dish_Price = @price, Allergen_Notes = @notes, " +
                           "Availability_Status = @status, Dish_Type = @type WHERE Dish_ID = @id";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", d.Dish_Name),
                new MySqlParameter("@price", d.Dish_Price),
                new MySqlParameter("@notes", d.Allergen_Notes),
                new MySqlParameter("@status", d.Availability_Status),
                new MySqlParameter("@type", d.Dish_Type?.DishType_Name), 
                new MySqlParameter("@id", d.Dish_ID)
            ) > 0;
        }
        #endregion

        #region Update Dish Availability Status
        public void UpdateDishesAvailibility()
        {
            string query = @"
                UPDATE dishes d
                SET d.Availability_Status = 0
                WHERE EXISTS (
                    SELECT 1
                    FROM products_in_dish pid
                    JOIN products p ON pid.Product_ID = p.Product_ID
                    WHERE pid.Dish_ID = d.Dish_ID
                      AND (p.Quantity_Available < pid.Amount_Usage OR p.Quantity_Available <= d.Tolerance)
                );
            ";

            _db.ExecuteNonQuery(query);
        }

        #endregion
    }
}
