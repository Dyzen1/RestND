public class Transaction{
readonly DbConnection _db;
public Transaction(DbConnection db)
{
    _db = db;
}
#region Delete and Add Dish
public override bool Delete(int dishId)
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



public override bool Add(Dish d)
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
        bool productsAdded = productInDishService.AddProductsToDish(newDishId, d.ProductUsage, _db.Connection, transaction);

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
    



}