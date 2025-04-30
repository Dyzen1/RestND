using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using System.Collections.Generic;

namespace RestND.Data
{
    // Service to handle inserting the products used in a dish into the 'product_in_dish' table
    public class ProductInDishService
    {
        private readonly DatabaseOperations _db = DatabaseOperations.Instance;

        
        public bool AddProductsToDish(int dishId, List<ProductUsageInDish> productUsages)
        {
            var affectedRows = 0;


            var query = "INSERT INTO product_in_dish (Dish_ID, Product_ID, Amount_Usage) VALUES (@dishId, @productId, @amount)";
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



        // Delete a specific product from a specific dish
        public bool DeleteProductFromDish(int dishId, int productId)
        {
            var query = "DELETE FROM product_in_dish WHERE Dish_ID = @dishId AND Product_ID = @productId";

            var affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@dishId", dishId),
                new MySqlParameter("@productId", productId)
            );

            return affectedRows > 0;
        }


        // Update a product usage in a dish
        public bool UpdateProductInDish(int dishId, int productId, double newAmountUsage)
        {
            var query = "UPDATE product_in_dish " +
                        "SET Amount_Usage = @amount " +
                        "WHERE Dish_ID = @dishId AND Product_ID = @productId";

            var affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@amount", newAmountUsage),
                new MySqlParameter("@dishId", dishId),
                new MySqlParameter("@productId", productId)
            );

            return affectedRows > 0;
        }
    }
}