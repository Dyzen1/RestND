using MySql.Data.MySqlClient;
using RestND.MVVM.Model.Orders;

namespace RestND.Data
{
    public class DishInOrderServices
    {
        private readonly DatabaseOperations _db;

        #region Constructors
        // default: singleton DB instance
        public DishInOrderServices() : this(DatabaseOperations.Instance) { }

        // injected: reuse an existing DatabaseOperations (same conn/tx)
        public DishInOrderServices(DatabaseOperations db)
        {
            _db = db;
        }
        #endregion

        #region Add Dish to Order
        // Transactional insert (use SAME connection + transaction)
        public bool AddDishToOrder(int orderId, DishInOrder dish, MySqlConnection conn, MySqlTransaction tx)
        {
            const string sql =
                "INSERT INTO dishes_in_order (Order_ID, Dish_ID, Quantity, Price) " +
                "VALUES (@orderId, @dishId, @qty, @price)";

            return _db.ExecuteNonQuery(sql, conn, tx,
                new MySqlParameter("@orderId", orderId),
                new MySqlParameter("@dishId", dish.dish.Dish_ID),
                new MySqlParameter("@qty", dish.Quantity),
                new MySqlParameter("@price", dish.dish.Dish_Price)
            ) > 0;
        }

        // Non-transactional insert
        public bool AddDishToOrder(int orderId, DishInOrder dish)
        {
            const string sql =
                "INSERT INTO dishes_in_order (Order_ID, Dish_ID, Dish_Name, Quantity, Total_Dish_Price) " +
                "VALUES (@orderId, @dishId, @dishName, @qty, @total)";

            return _db.ExecuteNonQuery(sql,
                new MySqlParameter("@orderId", orderId),
                new MySqlParameter("@dishId", dish.dish.Dish_ID),
                new MySqlParameter("@dishName", dish.dish.Dish_Name),
                new MySqlParameter("@qty", dish.Quantity),
                new MySqlParameter("@total", dish.TotalDishPrice)
            ) > 0;
        }
        #endregion

        #region Delete Dish from Order
        public bool DeleteDishFromOrder(int dishId, int orderId)
        {
            const string query = "DELETE FROM dishes_in_order WHERE Dish_ID = @dishId AND Order_ID = @orderId";
            var affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@dishId", dishId),
                new MySqlParameter("@orderId", orderId));
            return affectedRows > 0;
        }
        #endregion

        #region Update Dish in Order
        public bool UpdateDishInOrder(int orderId, DishInOrder d)
        {
            const string query =
                "UPDATE dishes_in_order SET Total_Dish_Price = @total, Quantity = @quantity WHERE Dish_ID = @dishId AND Order_ID = @orderId";
            var affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@quantity", d.Quantity),
                new MySqlParameter("@dishId", d.dish.Dish_ID),
                new MySqlParameter("@orderId", orderId),
                new MySqlParameter("@total", d.TotalDishPrice));
            return affectedRows > 0;
        }
        #endregion
    }
}
