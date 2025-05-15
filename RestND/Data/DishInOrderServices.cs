using MySql.Data.MySqlClient;
using RestND.MVVM.Model.Employees;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using System.Collections.Generic;

namespace RestND.Data
{
    public class DishInOrderServices
    {
        private readonly DatabaseOperations _db = DatabaseOperations.Instance;

        // Add dishes to order
        #region Add Dishes to order
        public bool AddDishToOrder(string orderId, DishInOrder dishInOrder)
        {
            var affectedRows = 0;
            var query = "INSERT INTO dishes_in_order(Order_ID, Dish_ID, Quantity) VALUES (@order_id, @dish_id, @quantity)";

            affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@order_id", orderId),
                new MySqlParameter("@quantity", dishInOrder.Quantity),
                new MySqlParameter("@dish_id", dishInOrder.Dish.Dish_ID));

            return affectedRows > 0;
        }
        #endregion

        // Delete a specific dish from an order
        #region Delete Dish from Order
        public bool DeleteDishFromOrder(int dishId, int orderId)
        {
            var query = "DELETE FROM dishes_in_order WHERE Dish_ID = @dishId AND Order_ID = @orderId";

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
            var query = "UPDATE dishes_in_order " +
                        "SET Quantity = @quantity " +
                        "WHERE Dish_ID = @dishId AND Order_ID = @orderId";

            var affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@quantity", newDishAmount),
                new MySqlParameter("@dishId", dishId),
                new MySqlParameter("@orderId", orderId)
            );

            return affectedRows > 0;
        }
        #endregion
    }
}
