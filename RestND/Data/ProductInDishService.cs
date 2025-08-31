using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;

namespace RestND.Data
{
    // Service to handle inserting the products used in a dish into the 'product_in_dish' table
    public class ProductInDishService
    {
        private readonly DatabaseOperations _db = DatabaseOperations.Instance;

        //#region Get All Products in Dish
        //public List<ProductInDish> GetAll()
        //{
        //    var products = new List<ProductInDish>();
        //    var query = "SELECT * FROM products_in_dish";
        //    var rows = _db.ExecuteReader(query);

        //    foreach (var row in rows)
        //    {
        //        products.Add(new ProductInDish
        //        {
        //            Product_ID = row["Product_ID"].ToString(),
        //            Product_Name = row["Product_Name"].ToString(),
        //            Amount_Usage = Convert.ToDouble(row["Amount_Usage"]),
        //            Dish_ID = Convert.ToInt32(row["Dish_ID"])
        //        });
        //    }
        //    return products;
        //}

        //#endregion

        #region Get Products in Dish by Dish ID
        public List<ProductInDish> GetProductsInDish(int dishId)
        {
            var products = new List<ProductInDish>();

            // Only load active product links
            var query = @"SELECT pid.Dish_ID, pid.Product_ID, pid.Amount_Usage, pid.Is_Active, i.Product_Name
                  FROM products_in_dish pid
                  JOIN inventory i ON i.Product_ID = pid.Product_ID
                  WHERE pid.Dish_ID = @dishId AND pid.Is_Active = 1";

            var rows = _db.ExecuteReader(query, new MySqlParameter("@dishId", dishId));
            foreach (var row in rows)
            {
                products.Add(new ProductInDish
                {
                    Dish_ID = Convert.ToInt32(row["Dish_ID"]),
                    Product_ID = Convert.ToString(row["Product_ID"]) ?? string.Empty,
                    Product_Name = Convert.ToString(row["Product_Name"]) ?? string.Empty,
                    Amount_Usage = row["Amount_Usage"] != DBNull.Value ? Convert.ToDouble(row["Amount_Usage"]) : 0.0,
                    Is_Active = row["Is_Active"] != DBNull.Value && Convert.ToBoolean(row["Is_Active"])
                });
            }
            return products;
        }
        #endregion

        // Add products to a dish
        #region Add Products to Dish
        public bool AddProductsToDish(int dishId, List<ProductInDish> productUsages)
        {
            var affectedRows = 0;

            var query = "INSERT INTO products_in_dish (Dish_ID, Product_ID, Amount_Usage) VALUES (@dishId, @productId, @amount)";
            foreach (var usage in productUsages)
            {
                affectedRows = _db.ExecuteNonQuery(query,
                    new MySqlParameter("@dishId", dishId),
                    new MySqlParameter("@productId", usage.Product_ID),
                    new MySqlParameter("@amount", usage.Amount_Usage)
                );

            }

            return affectedRows > 0;
        }
        #endregion

        // Delete a specific product from a specific dish
        #region Delete Product from Dish
        public bool DeleteProductFromDish(int dishId, int productId)
        {
            var query = "DELETE FROM products_in_dish WHERE Product_ID = @productId";

            var affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@dishId", dishId),
                new MySqlParameter("@productId", productId)
            );

            return affectedRows > 0;
        }
        #endregion


        // soft delete product from db


        #region soft delete product from product in dish

        public int DeleteProductEverywhere(string productId)
        {
            var query = "UPDATE products_in_dish SET Is_Active = false WHERE Product_ID = @productId";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@productId", productId));
        }
        #endregion


        // Update a product usage in a dish
        #region Update Product in Dish
        public bool UpdateProductInDish(int dishId, int productId, double newAmountUsage)
        {
            var query = "UPDATE products_in_dish " +
                        "SET Amount_Usage = @amount " +
                        "WHERE Dish_ID = @dishId AND Product_ID = @productId";

            var affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@amount", newAmountUsage),
                new MySqlParameter("@dishId", dishId),
                new MySqlParameter("@productId", productId)
            );

            return affectedRows > 0;
        }
        #endregion
    }
}