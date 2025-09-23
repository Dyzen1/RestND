using MySql.Data.MySqlClient;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using RestND.Validations;
using System;
using System.Collections.Generic;

public class DiscountService() : BaseService<Discount>(DatabaseOperations.Instance)
{

    #region Get All Discounts
    public override List <Discount> GetAll()
    {
        var discounts = new List<Discount>();
        string query = "SELECT * FROM discounts";
        var rows = _db.ExecuteReader(query);


        foreach (var row in rows)
        {
            if(row.TryGetValue("Is_Active", out var isActive) && Convert.ToBoolean(isActive) == false)
                continue; // Skip inactive discounts
            discounts.Add(new Discount
            {
                Discount_ID = Convert.ToInt32(row["Discount_ID"]),
                Discount_Name = row["Discount_Name"].ToString(),
                Discount_Percentage = Convert.ToDouble(row["Discount_Percentage"])
            });
        }

        return discounts;
    }

    #endregion

    #region Add Discount
    public override bool Add (Discount d)
    {
        string query = "INSERT INTO discounts (Discount_Name, Discount_Percentage) VALUES (@name, @percentage)";
        
        return _db.ExecuteNonQuery(query,
            new MySqlParameter("@name", d.Discount_Name),
            new MySqlParameter("@percentage", d.Discount_Percentage)) > 0;  
        
    }
    #endregion

    #region Update Discount
    public override bool Update (Discount d){
        string query = "UPDATE discounts SET Discount_Name = @name, Discount_Percentage = @percentage WHERE Discount_ID = @id";
        return _db.ExecuteNonQuery(query,
            new MySqlParameter("@name", d.Discount_Name),
            new MySqlParameter("@percentage", d.Discount_Percentage),
            new MySqlParameter("@id", d.Discount_ID)) > 0;  
        
    }
    #endregion

    #region Delete Discount (not really deleting, just marking as inactive)
    public override bool Delete(Discount d)
    {
        d.Is_Active = false; 
        string query = "UPDATE discounts SET Is_Active = @active WHERE Discount_ID = @id";
        return _db.ExecuteNonQuery(query,
            new MySqlParameter("@active", d.Is_Active),
            new MySqlParameter("@id", d.Discount_ID)) > 0;
    }

    #endregion


}
