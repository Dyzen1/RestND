using MySql.Data.MySqlClient;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using RestND.MVVM.View.Windows;
using System;
using System.Windows;

public class Transaction
{
    #region Properties
    readonly DatabaseOperations _db;
    #endregion

    #region Constructor


    public Transaction(DatabaseOperations db)
    {
        _db = db;
        
    }
    #endregion

    #region Add Dish or Update Dish and Update Product in Dish
    public bool AddDish(Dish d)
    {

        _db.OpenConnection();
        using var transaction = _db.Connection.BeginTransaction();
       
        try
        {
            string query = "INSERT INTO dishes (Dish_Name, Dish_Price, Allergen_Notes, DishType_Name, Is_Active) " +
                           "VALUES (@name, @price, @notes, @type, @active)";
          
            bool dishAdded = _db.ExecuteNonQuery(query, _db.Connection, transaction,
                new MySqlParameter("@name", d.Dish_Name),
                new MySqlParameter("@price", d.Dish_Price),
                new MySqlParameter("@notes", d.Allergen_Notes),
                new MySqlParameter("@type", d.Dish_Type.DishType_Name),
                new MySqlParameter("@active", d.Is_Active)
            ) > 0;

            if (!dishAdded)
            {
                transaction.Rollback();
                return false;
            }
      

            int newDishId = Convert.ToInt32(_db.ExecuteScalar("SELECT LAST_INSERT_ID();", _db.Connection, transaction));

            var productInDishService = new ProductInDishService();
            bool productsAdded = productInDishService.AddProductsToDish(_db.Connection,transaction,newDishId, d.ProductUsage);

            if (!productsAdded)
            {
                transaction.Rollback();
                return false;
            }

            transaction.Commit(); 
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error adding dish: " + ex.Message, "Database Error");
            return false;
        }
        finally
        {
            _db.Connection.Close();
        }
    }



    public bool UpdateDish(Dish dish)
    {
        _db.OpenConnection();
        using var transaction = _db.Connection.BeginTransaction();

        try
        {
            string query = @"
            UPDATE dishes 
            SET Dish_Name = @name, 
                Dish_Price = @price, 
                Allergen_Notes = @notes,
                DishType_Name = @type
                WHERE Dish_ID = @id";

            _db.ExecuteNonQuery(query, _db.Connection, transaction,
                new MySqlParameter("@name", dish.Dish_Name),
                new MySqlParameter("@price", dish.Dish_Price),
                new MySqlParameter("@notes", string.Join(",", dish.Allergen_Notes)),
                new MySqlParameter("@status", dish.Is_Active),
                new MySqlParameter("@type", dish.Dish_Type?.DishType_Name ?? string.Empty),
                new MySqlParameter("@id", dish.Dish_ID)
            );
            //delete all existing product in dish for the update.
            string deleteQuery = "DELETE FROM products_in_dish WHERE Dish_ID = @id";
            _db.ExecuteNonQuery(deleteQuery, _db.Connection, transaction,
                new MySqlParameter("@id", dish.Dish_ID)
            );
            //insert all the product in dish from the updated (new products in dish).
            foreach (var usage in dish.ProductUsage)
            {
                string insertQuery = "INSERT INTO products_in_dish (Dish_ID, Product_ID, Amount_Usage) VALUES (@dishId, @productId, @amount)";
                _db.ExecuteNonQuery(insertQuery, _db.Connection, transaction,
                    new MySqlParameter("@dishId", dish.Dish_ID),
                    new MySqlParameter("@productId", usage.Product_ID),
                    new MySqlParameter("@amount", usage.Amount_Usage)
                );
            }

            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error updating dish: " + ex.Message, "Database Error");
            return false;
        }
        finally
        {
            _db.Connection.Close();
        }
    }


    #endregion

    #region Delete and Add Order
    public bool AddOrder(Order o)
    {
        // ---- Guards (quick feedback before touching the DB) ----
        if (o == null || o.Table == null || o.assignedEmployee == null)
            throw new ArgumentException("Order, Table, and AssignedEmployee must be provided.");

        if (o.People_Count <= 0)
            throw new ArgumentException("People_Count must be a positive number.");

        if (o.Table.Max_Diners > 0 && o.People_Count > o.Table.Max_Diners)
            throw new ArgumentException($"People_Count ({o.People_Count}) exceeds table capacity ({o.Table.Max_Diners}).");

        _db.OpenConnection();
        using var transaction = _db.Connection.BeginTransaction();

        try
        {
            // 1) Insert the Order (NOTE: using your existing column name 'Price')
            string insertOrderSql =
                "INSERT INTO orders (Employee_Name, Table_Number, People_Count, Price, Is_Active) " +
                "VALUES (@name, @number, @people, @price, @active)";

            bool orderAdded = _db.ExecuteNonQuery(insertOrderSql, _db.Connection, transaction,
                new MySqlParameter("@name", o.assignedEmployee.Employee_Name),
                new MySqlParameter("@number", o.Table.Table_Number),
                new MySqlParameter("@people", o.People_Count),
                new MySqlParameter("@price", o.Bill?.Price ?? 0.0),
                new MySqlParameter("@active", o.Is_Active)
            ) > 0;

            if (!orderAdded)
            {
                transaction.Rollback();
                _db.CloseConnection();
                return false;
            }

            // 2) Get the new Order ID
            int newOrderId = Convert.ToInt32(
                _db.ExecuteScalar("SELECT LAST_INSERT_ID();", _db.Connection, transaction)
            );

            // 3) Insert Dishes for this order (inside the SAME transaction)
            var dishInOrderServices = new DishInOrderServices(_db); // ensure this reuses the SAME _db

            if (o.DishInOrder != null && o.DishInOrder.Count > 0)
            {
                foreach (var dish in o.DishInOrder)
                {
                    // requires an overload that accepts the existing connection+transaction (see below)
                    bool dishesAdded = dishInOrderServices.AddDishToOrder(newOrderId, dish, _db.Connection, transaction);
                    if (!dishesAdded)
                    {
                        transaction.Rollback();
                        _db.CloseConnection();
                        return false;
                    }
                }
            }

            // 4) Commit transaction if everything succeeded
            transaction.Commit();
            _db.CloseConnection();
            return true;
        }
        catch
        {
            transaction.Rollback();
            _db.CloseConnection();
            throw;
        }
    }



    public bool DeleteOrder(int orderId)
    {
        if (orderId <= 0) return false;

        _db.OpenConnection();
        try
        {
            // Safety check — if the order has dishes, do a soft delete instead
            var hasDishes = Convert.ToInt32(_db.ExecuteScalar(
                "SELECT COUNT(*) FROM dishes_in_order WHERE Order_ID = @id",
                _db.Connection, null,
                new MySqlParameter("@id", orderId)
            )) > 0;

            if (hasDishes)
            {
                // Soft delete (match your app-wide pattern)
                const string softDelete = "UPDATE orders SET Is_Active = 0 WHERE Order_ID = @id";
                return _db.ExecuteNonQuery(softDelete, _db.Connection, null,
                    new MySqlParameter("@id", orderId)
                ) > 0;
            }

            // No dishes ? do a hard delete in a single transaction
            using var tx = _db.Connection.BeginTransaction();
            try
            {
                const string deleteDishes = "DELETE FROM dishes_in_order WHERE Order_ID = @id";
                _db.ExecuteNonQuery(deleteDishes, _db.Connection, tx, new MySqlParameter("@id", orderId));

                const string deleteOrder = "DELETE FROM orders WHERE Order_ID = @id";
                int rows = _db.ExecuteNonQuery(deleteOrder, _db.Connection, tx, new MySqlParameter("@id", orderId));

                if (rows == 0)
                {
                    tx.Rollback();
                    return false;
                }

                tx.Commit();
                return true;
            }
            catch
            {
                // Attempt to roll back and rethrow
                try { tx.Rollback(); } catch { /* ignore */ }
                throw;
            }
        }
        finally
        {
            _db.CloseConnection();
        }
    }

    #endregion

    #region soft delete product from everywhere


    #endregion
}