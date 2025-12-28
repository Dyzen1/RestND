using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using RestND.MVVM.Model.Employees;
using System;
using System.Collections.Generic;

namespace RestND.Data
{
    class SoftDrinkServices() : BaseService<SoftDrink>(DatabaseOperations.Instance)
    {
        #region Get all SoftDrinks
        public override List<SoftDrink> GetAll()
        {
            var drinks = new List<SoftDrink>();
            const string query = @"
                SELECT Drink_ID, Drink_Name, Drink_Price, Quantity, Is_Active, DishType
                FROM soft_drinks
                WHERE Is_Active = TRUE AND DishType = 'SoftDrinks';";

            var rows = _db.ExecuteReader(query);
            foreach (var row in rows)
            {
                drinks.Add(new SoftDrink
                {
                    Drink_ID = Convert.ToInt32(row["Drink_ID"]),
                    Drink_Name = Convert.ToString(row["Drink_Name"]),
                    Drink_Price = Convert.ToDouble(row["Drink_Price"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    Is_Active = Convert.ToBoolean(row["Is_Active"]),
                });
            }
            return drinks;
        }
        #endregion

        #region Add SoftDrink
        public override bool Add(SoftDrink d)
        {
            const string query = @"
            INSERT INTO soft_drinks
                (Drink_Name, Drink_Price, Quantity, Is_Active, DishType)
            VALUES
                (@name, @price, @qty, @active, @dishType);";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", (object?)d.Drink_Name ?? DBNull.Value),
                new MySqlParameter("@price", d.Drink_Price),
                new MySqlParameter("@qty", d.Quantity),
                new MySqlParameter("@active", d.Is_Active),
                new MySqlParameter("@dishType", "SoftDrinks") // force it here
            ) > 0;
        }
        #endregion

        #region Update
        public override bool Update(SoftDrink d)
        {
            const string query = @"
            UPDATE soft_drinks
            SET Drink_Name = @name,
                Drink_Price = @price,
                Quantity    = @qty,
                DishType    = @dishType
            WHERE Drink_ID = @id;";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", d.Drink_ID),
                new MySqlParameter("@name", (object?)d.Drink_Name ?? DBNull.Value),
                new MySqlParameter("@price", d.Drink_Price),
                new MySqlParameter("@qty", d.Quantity),
                new MySqlParameter("@dishType", "SoftDrinks")
            ) > 0;
        }
        #endregion

        #region Soft Delete (flip Is_Active to false)
        public override bool Delete(SoftDrink d)
        {
            d.Is_Active = false;

            const string query = "UPDATE soft_drinks SET Is_Active = @active WHERE Drink_ID = @id;";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@active", d.Is_Active),
                new MySqlParameter("@id", d.Drink_ID)
            ) > 0;
        }
        #endregion
    }
}
