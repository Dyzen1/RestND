using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Data
{
    public class DishInOrderServices
    {
        private readonly DatabaseOperations _db = DatabaseOperations.Instance;

        // Add dishes to order
        #region Add dishes to order
        public bool AddDishesToOrder(int orderId, List<DishInOrder> dishInOrder)
        {
            var affectedRows = 0;
            var query = "INSERT INTO dish_in_order(Order_ID, Dish_Name, Amount) VALUES (@orderId, @name, @amount)";
            foreach (var dish in dishInOrder)
            {
                affectedRows = _db.ExecuteNonQuery(query,
                    new MySqlParameter("@orderId", orderId),
                    new MySqlParameter("@name", dish.Dish_Name),
                    new MySqlParameter("@amount", dish.Amount)
                );

            }

            return affectedRows > 0;
        }
        #endregion

        // Delete a specific dish from an order
        #region Delete Dish from Order
        public bool DeleteDishFromOrder(int dishId, int orderId)
        {
            var query = "DELETE FROM dish_in_order WHERE Dish_ID = @dishId AND Order_ID = @orderId";

            var affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@dishId", dishId),
                new MySqlParameter("@orderId", orderId)
            );

            return affectedRows > 0;
        }
        #endregion

        // Update a dish amount of a dish
        #region Update Dish in amount
        public bool UpdateProductInDish(int dishId, int orderId, int newDishAmount)
        {
            var query = "UPDATE dish_in_order " +
                        "SET Amount = @amount " +
                        "WHERE Dish_ID = @dishId AND Order_ID = @orderId";

            var affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@amount", newDishAmount),
                new MySqlParameter("@dishId", dishId),
                new MySqlParameter("@orderId", orderId)
            );

            return affectedRows > 0;
        }
        #endregion
    }
}
