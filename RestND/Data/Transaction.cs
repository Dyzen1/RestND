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
                _db.CloseConnection();
                return false;
            }

            int newDishId = Convert.ToInt32(_db.ExecuteScalar("SELECT LAST_INSERT_ID();", _db.Connection, transaction));

            var productInDishService = new ProductInDishService();
            bool productsAdded = productInDishService.AddProductsToDish(newDishId, d.ProductUsage);

            if (!productsAdded)
            {
                transaction.Rollback();
                _db.CloseConnection();
                return false;
            }

            transaction.Commit();
            _db.CloseConnection();
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _db.CloseConnection();
            System.Windows.MessageBox.Show("Error adding dish: " + ex.Message, "Database Error");
            return false;
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
            _db.CloseConnection();
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _db.CloseConnection();
            MessageBox.Show("Error updating dish: " + ex.Message, "Database Error");
            return false;
        }
    }


    #endregion

    #region Delete and Add Order
    public bool DeleteOrder(int orderId)
    {
        _db.OpenConnection();
        using var transaction = _db.Connection.BeginTransaction();

        try
        {
            // 1. Delete from child table first
            string deleteDishesQuery = "DELETE FROM dishes_in_order WHERE Order_ID = @id";
            _db.ExecuteNonQuery(deleteDishesQuery, _db.Connection, transaction,
                new MySqlParameter("@id", orderId)
            );

            // 2. Delete from parent table
            string deleteOrderQuery = "DELETE FROM orders WHERE Order_ID = @id";
            int rowsAffected = _db.ExecuteNonQuery(deleteOrderQuery, _db.Connection, transaction,
                new MySqlParameter("@id", orderId)
            );

            if (rowsAffected == 0)
            {
                transaction.Rollback();
                _db.CloseConnection();
                return false;
            }

            transaction.Commit();
            _db.CloseConnection();
            return true;
        }
        catch (Exception)
        {
            transaction.Rollback();
            _db.CloseConnection();
            throw;
        }
    }

    public bool AddOrder(Order o)
    {
        _db.OpenConnection();
        using var transaction = _db.Connection.BeginTransaction();

        try
        {
            // 1. Insert the Order
            string query = "INSERT INTO orders (Employee_Name, Table_Number, Price) " +
                           "VALUES (@name, @number, @price)";

            bool orderAdded = _db.ExecuteNonQuery(query, _db.Connection, transaction,
                new MySqlParameter("@name", o.assignedEmployee.Employee_Name),
                new MySqlParameter("@number", o.Table.Table_Number),
                new MySqlParameter("@price", o.Bill.Price)
            ) > 0;

            if (!orderAdded)
            {
                transaction.Rollback();
                _db.CloseConnection();
                return false;
            }

            // 2. Get the new Order ID
            int newOrderId = Convert.ToInt32(_db.ExecuteScalar("SELECT LAST_INSERT_ID();", _db.Connection, transaction));

            // 3. Insert Dishes 
            var dishInOrderServices = new DishInOrderServices();
            foreach(var dish in o.DishInOrder)
            {
                bool dishesAdded = dishInOrderServices.AddDishToOrder(newOrderId, dish);
                if (!dishesAdded)
                {
                    transaction.Rollback();
                    _db.CloseConnection();
                    return false;
                }
            }

            // 4. Commit transaction if everything succeeded
            transaction.Commit();
            _db.CloseConnection();
            return true;
        }
        catch (Exception)
        {
            transaction.Rollback();
            _db.CloseConnection();
            throw;
        }
    }
    #endregion


    #region soft delete product from everywhere


    #endregion
}