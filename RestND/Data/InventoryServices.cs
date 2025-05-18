using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;

namespace RestND.Data
{
    public class ProductService() : BaseService<Inventory>(DatabaseOperations.Instance)
    {
        #region Get All Products
        public override List<Inventory> GetAll()
        {
            var products = new List<Inventory>();
            string query = "SELECT * FROM inventory";
            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                products.Add(new Inventory
                {
                    Product_ID = row["Product_ID"].ToString(),      
                    Product_Name = row["Product_Name"].ToString(),
                    Quantity_Available = Convert.ToInt32(row["Quantity_Available"])
                });
            }

            return products;
        }

        #endregion

        #region Add Product
        public override bool Add(Inventory p)
        {
            string query = "INSERT INTO inventory (Product_Name, Quantity_Available) VALUES (@name, @qty)";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", p.Product_Name),
                new MySqlParameter("@qty", p.Quantity_Available)) > 0;
        }
        #endregion

        #region Update Product
        public override bool Update(Inventory p)
        {
            string query = "UPDATE inventory SET Product_Name = @name, Quantity_Available = @qty WHERE Product_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", p.Product_Name),
                new MySqlParameter("@qty", p.Quantity_Available),
                new MySqlParameter("@id", p.Product_ID)) > 0;
        }
        #endregion

        #region Delete Product (accepts int)
        public override bool Delete(int productId)
        { 
            string query = "DELETE FROM inventory WHERE Product_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", productId)) > 0;
        }

        #endregion

        #region Delete Product (accepts string)
        public bool Delete(string productId)
        {
            string query = "DELETE FROM inventory WHERE Product_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", productId)) > 0;
        }

        #endregion
    }

}
