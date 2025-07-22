﻿using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
            string query = "SELECT * FROM dishes WHERE Is_Active = 1";
            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                //if (row.TryGetValue("Is_Active", out var isActive) && Convert.ToBoolean(isActive) == false)
                //    continue; // Skip inactive dishes

                var dish = new Dish
                {
                    Dish_ID = Convert.ToInt32(row["Dish_ID"]),
                    Dish_Name = row["Dish_Name"].ToString(),
                    Dish_Price = Convert.ToInt32(row["Dish_Price"]),
                    Dish_Type = new DishType
                    {
                        DishType_Name = row["DishType_Name"].ToString()
                    }
                };

                // Handle Allergen_Notes if present and not null
                if (row.TryGetValue("Allergen_Notes", out var allergenNotes) && allergenNotes != null)
                {
                    dish.Allergen_Notes = allergenNotes.ToString();
                }

                dishes.Add(dish);
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

        #region Delete Dish (not really deleting, just marking as inactive)
        public override bool Delete(Dish d)
        {
            d.Is_Active = false;
            string query = "UPDATE dishes SET Is_Active = @active WHERE Dish_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@active", d.Is_Active),
                new MySqlParameter("@id", d.Dish_ID)) > 0;
        }

        #endregion

        #region Full Update Dish
        public override bool Update(Dish d)
        {   
            return _transaction.UpdateDish(d);
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
