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

        #region Get All Products in Dish
        public List<ProductInDish> GetAll()
        {
            var products = new List<ProductInDish>();
            var query = "SELECT * FROM products_in_dish";
            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                products.Add(new ProductInDish
                {
                    Product_ID = row["Product_ID"].ToString(),
                    Product_Name = row["Product_Name"].ToString(),
                    Amount_Usage = Convert.ToDouble(row["Amount_Usage"]),
                    Dish_ID = Convert.ToInt32(row["Dish_ID"])


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
            var query = "DELETE FROM products_in_dish WHERE Dish_ID = @dishId AND Product_ID = @productId";

            var affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@dishId", dishId),
                new MySqlParameter("@productId", productId)
            );

            return affectedRows > 0;
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