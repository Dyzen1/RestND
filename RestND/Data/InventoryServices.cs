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
                if (row.TryGetValue("Is_Active", out var isActive) && Convert.ToBoolean(isActive) == false)
                    continue; // Skip inactive products
                products.Add(new Inventory
                {
                    Product_ID = row["Product_ID"].ToString(),
                    Product_Name = row["Product_Name"].ToString(),
                    Tolerance = row["Tolerance"] != DBNull.Value ? Convert.ToDouble(row["Tolerance"]) : 0.0,
                    Quantity_Available = Convert.ToInt32(row["Quantity_Available"]),
                    Created_At = row["Created_At"].ToString().Split(' ')[0]
                });
            }

            return products;
        }

        #endregion

        #region Add Product
        public override bool Add(Inventory p)
        {
            string query = "INSERT INTO inventory (Product_ID, Product_Name, Tolerance, Quantity_Available, Is_Active) VALUES (@id, @name, @tolerance, @qty, @active)";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@id", p.Product_ID),
                new MySqlParameter("@name", p.Product_Name),
                new MySqlParameter("@tolerance", p.Tolerance),
                new MySqlParameter("@active", p.Is_Active),
                new MySqlParameter("@qty", p.Quantity_Available)) > 0;
        }
        #endregion

        #region Create a dictionary of products where key is the product ID and value is the quantity
        public Dictionary<string, double> GetProductDictionary()
        {
            var productDictionary = new Dictionary<string, double>();
            string query = "SELECT Product_ID, Quantity_Available FROM inventory";
            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                string productId = row["Product_ID"].ToString();
                double quantity = Convert.ToDouble(row["Quantity_Available"]);

                if (!productDictionary.ContainsKey(productId))
                {
                    productDictionary.Add(productId, quantity);
                }
                
            }

            return productDictionary;
        }
        #endregion

        #region Update Product Quantity 
        public bool UpdateProductQuantity(string productId, double newQuantity)
        {
            string query = "UPDATE inventory SET Quantity_Available = @quantity WHERE Product_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@quantity", newQuantity),
                new MySqlParameter("@id", productId)) > 0;
        }

        #endregion

        #region Update Product
        public override bool Update(Inventory p)
        {
            string query = "UPDATE inventory SET Product_Name = @name, Quantity_Available = @qty,Tolerance = @tolerance WHERE Product_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", p.Product_Name),
                new MySqlParameter("@qty", p.Quantity_Available),
                new MySqlParameter("@tolerance", p.Tolerance),
                new MySqlParameter("@id", p.Product_ID)) > 0;
        }
        #endregion

        #region Delete Product (accepts int)
        public override bool Delete(Inventory d)
        {
            d.Is_Active = false;
            string query = "UPDATE inventory SET Is_Active = @active WHERE Product_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@active", d.Is_Active),
                new MySqlParameter("@id", d.Product_ID)) > 0;
        }

        #endregion
    }

}
