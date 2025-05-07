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
    #region Delete and Add Dish
    public  bool DeleteDish(string dishId)
    {
        _db.OpenConnection();
        using var transaction = _db.Connection.BeginTransaction();

        try
        {
            // 1. Delete from child table first
            string deleteProductsQuery = "DELETE FROM product_in_dish WHERE Dish_ID = @id";
            _db.ExecuteNonQuery(deleteProductsQuery, _db.Connection, transaction,
                new MySqlParameter("@id", dishId)
            );

            // 2. Delete from parent table
            string deleteDishQuery = "DELETE FROM dish WHERE Dish_ID = @id";
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
            // 1. Insert the Dish
            string query = "INSERT INTO dish (Dish_Name, Dish_Price, Allergen_Notes, Availability_Status, Dish_Type) " +
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

            // 2. Get the new Dish ID
            int newDishId = Convert.ToInt32(_db.ExecuteScalar("SELECT LAST_INSERT_ID();", _db.Connection, transaction));

            // 3. Insert ProductUsage (always required)
            var productInDishService = new ProductInDishService();
            bool productsAdded = productInDishService.AddProductsToDish(newDishId, d.ProductUsage);

            if (!productsAdded)
            {
                transaction.Rollback();
                _db.CloseConnection();
                return false;
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

    #region Delete and Add Order

    public bool DeleteOrder(string orderId)
    {
        _db.OpenConnection();
        using var transaction = _db.Connection.BeginTransaction();

        try
        {
            // 1. Delete from child table first
            string deleteDishesQuery = "DELETE FROM dish_in_order WHERE Order_ID = @id";
            _db.ExecuteNonQuery(deleteDishesQuery, _db.Connection, transaction,
                new MySqlParameter("@id", orderId)
            );

            // 2. Delete from parent table
            string deleteOrderQuery = "DELETE FROM order WHERE Order_ID = @id";
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
            string query = "INSERT INTO order (Employee_Name, Table_Number, Price) " +
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
            bool dishesAdded = dishInOrderServices.AddDishesToOrder(newOrderId, o.DishInOrder);

            if (!dishesAdded)
            {
                transaction.Rollback();
                _db.CloseConnection();
                return false;
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