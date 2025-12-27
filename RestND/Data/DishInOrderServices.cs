using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
<<<<<<< HEAD
using System.Windows;
=======
using System;
using System.Collections.Generic;
>>>>>>> e241a99 (added some stuff)

namespace RestND.Data
{
    public class DishInOrderServices
    {
        #region DB property
        private readonly DatabaseOperations _db;
        #endregion

        #region Constructors
        public DishInOrderServices() : this(DatabaseOperations.Instance) { }

        public DishInOrderServices(DatabaseOperations db)
        {
            _db = db;
        }
        #endregion

        #region Add Dish to Order

        // ✅ Transactional insert (same connection + transaction)
        public bool AddDishToOrder(int orderId, DishInOrder dish, MySqlConnection conn, MySqlTransaction tx)
        {
            const string sql =
                @"INSERT INTO dishes_in_order (Order_ID, Dish_ID, Quantity, Dish_Name, Total_Dish_Price)
                  VALUES (@orderId, @dishId, @qty, @dishName, @total)";

            return _db.ExecuteNonQuery(sql, conn, tx,
                new MySqlParameter("@orderId", orderId),
                new MySqlParameter("@dishId", dish.dish.Dish_ID),
                new MySqlParameter("@qty", dish.Quantity),
                new MySqlParameter("@dishName", dish.dish.Dish_Name),
                new MySqlParameter("@total", dish.TotalDishPrice)
            ) > 0;
        }

        // ✅ Non-transactional insert
        public bool AddDishToOrder(int orderId, DishInOrder dish)
        {
            const string sql =
<<<<<<< HEAD
                "INSERT INTO dishes_in_order (Order_ID, Dish_ID, Dish_Name, Quantity, Total_Dish_Price) " +
                "VALUES (@orderId, @dishId, @dishName, @qty, @total)";
            MessageBox.Show(dish.TotalDishPrice.ToString());
=======
                @"INSERT INTO dishes_in_order (Order_ID, Dish_ID, Quantity, Dish_Name, Total_Dish_Price)
                  VALUES (@orderId, @dishId, @qty, @dishName, @total)";

>>>>>>> e241a99 (added some stuff)
            return _db.ExecuteNonQuery(sql,
                new MySqlParameter("@orderId", orderId),
                new MySqlParameter("@dishId", dish.dish.Dish_ID),
                new MySqlParameter("@qty", dish.Quantity),
                new MySqlParameter("@dishName", dish.dish.Dish_Name),
                new MySqlParameter("@total", dish.TotalDishPrice)
            ) > 0;
        }

        #endregion

        #region Delete Dish from Order
        public bool DeleteDishFromOrder(int dishId, int orderId)
        {
            const string query =
                "DELETE FROM dishes_in_order WHERE Dish_ID = @dishId AND Order_ID = @orderId";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@dishId", dishId),
                new MySqlParameter("@orderId", orderId)
            ) > 0;
        }
        #endregion

        #region Update Dish in Order
        public bool UpdateDishInOrder(int orderId, DishInOrder d)
        {
            const string query =
                @"UPDATE dishes_in_order
                  SET Total_Dish_Price = @total,
                      Quantity = @quantity,
                      Dish_Name = @dishName
                  WHERE Dish_ID = @dishId AND Order_ID = @orderId";

            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@quantity", d.Quantity),
                new MySqlParameter("@dishId", d.dish.Dish_ID),
                new MySqlParameter("@orderId", orderId),
                new MySqlParameter("@total", d.TotalDishPrice),
                new MySqlParameter("@dishName", d.dish.Dish_Name)
            ) > 0;
        }
        #endregion

        public List<DishInOrder> GetByOrderId(int orderId)
        {
            const string sql = @"
        SELECT Dish_ID, Dish_Name, Quantity, Total_Dish_Price
        FROM dishes_in_order
        WHERE Order_ID = @id";

            var rows = _db.ExecuteReader(sql, new MySqlParameter("@id", orderId));
            var list = new List<DishInOrder>();

            foreach (var r in rows)
            {
                var dish = new Dish
                {
                    Dish_ID = Convert.ToInt32(r["Dish_ID"]),
                    Dish_Name = r["Dish_Name"].ToString()
                };

                list.Add(new DishInOrder(dish)
                {
                    Quantity = Convert.ToInt32(r["Quantity"]),
                    TotalDishPrice = Convert.ToInt32(r["Total_Dish_Price"])

                });
            }

            return list;
        }
    }

}
