using MySql.Data.MySqlClient;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using System;

public class Transaction{
readonly DatabaseOperations _db;
public Transaction(DatabaseOperations db)
{
    _db = db;
}
    #region Delete and Add Dish or Update Dish and Update Product in Dish
    public bool DeleteDish(int dishId)
    {
        _db.OpenConnection();
        using var transaction = _db.Connection.BeginTransaction();

        try
        {
          
            string deleteProductsQuery = "DELETE FROM products_in_dish WHERE Dish_ID = @id";
            _db.ExecuteNonQuery(deleteProductsQuery, _db.Connection, transaction,
                new MySqlParameter("@id", dishId)
            );

            
            string deleteDishQuery = "DELETE FROM dishes WHERE Dish_ID = @id";
            int rowsAffected = _db.ExecuteNonQuery(deleteDishQuery, _db.Connection, transaction,
                new MySqlParameter("@id", dishId)
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



    public bool AddDish(Dish d)
    {
        _db.OpenConnection();
        using var transaction = _db.Connection.BeginTransaction();

        try
        {
           
            string query = "INSERT INTO dishes (Dish_Name, Dish_Price, Allergen_Notes, Availability_Status, Dish_Type) " +
                           "VALUES (@name, @price, @notes, @status, @type)";

            bool dishAdded = _db.ExecuteNonQuery(query, _db.Connection, transaction,
                new MySqlParameter("@name", d.Dish_Name),
                new MySqlParameter("@price", d.Dish_Price),
                new MySqlParameter("@notes", d.Allergen_Notes),
                new MySqlParameter("@status", d.Availability_Status),
                new MySqlParameter("@type", d.Dish_Type.DishType_Name)
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
        catch (Exception)
        {
            transaction.Rollback();
            _db.CloseConnection();
            throw;
        }
    }




    public bool UpdateDish(Dish dish)
    {
        _db.OpenConnection();
        using var transaction = _db.Connection.BeginTransaction();

        try
        {
            // Update dish core info
            string query = @"
            UPDATE dishes 
            SET Dish_Name = @name, Dish_Price = @price, Allergen_Notes = @notes, 
                Availability_Status = @status, Dish_Type = @type, Tolerance = @tolerance
            WHERE Dish_ID = @id";

            _db.ExecuteNonQuery(query, _db.Connection, transaction,
                new MySqlParameter("@name", dish.Dish_Name),
                new MySqlParameter("@price", dish.Dish_Price),
                new MySqlParameter("@notes", string.Join(",", dish.Allergen_Notes)),
                new MySqlParameter("@status", dish.Availability_Status),
                new MySqlParameter("@type", dish.Dish_Type?.DishType_Name),
                new MySqlParameter("@id", dish.Dish_ID)
            );

            // Delete and re-insert products
            string deleteQuery = "DELETE FROM products_in_dish WHERE Dish_ID = @id";
            _db.ExecuteNonQuery(deleteQuery, _db.Connection, transaction,
                new MySqlParameter("@id", dish.Dish_ID)
            );

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
        catch
        {
            transaction.Rollback();
            _db.CloseConnection();
            throw;
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
                new MySqlParameter("@price", o.Table.Table_Number),
                new MySqlParameter("@notes", o.Bill.Price)
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

}