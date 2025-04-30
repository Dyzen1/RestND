using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;

namespace RestND.Data
{
    public class ProductService() : BaseService<Product>(DatabaseOperations.Instance)
    {
        #region Get All Products
        public override List<Product> GetAll()
        {
            var products = new List<Product>();
            string query = "SELECT * FROM product";
            var rows = _db.ExecuteReader(query);

            foreach (var row in rows)
            {
                products.Add(new Product
                {
                    Product_ID = Convert.ToInt32(row["Product_ID"]),
                    Product_Name = row["Product_Name"].ToString(),
                    Quantity_Available = Convert.ToInt32(row["Quantity_Available"])
                });
            }

            return products;
        }
        #endregion

        #region Add Product
        public override bool Add(Product p)
        {
            string query = "INSERT INTO product (Product_Name, Quantity_Available) VALUES (@name, @qty)";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", p.Product_Name),
                new MySqlParameter("@qty", p.Quantity_Available)) > 0;
        }
        #endregion

        #region Update Product
        public override bool Update(Product p)
        {
            string query = "UPDATE product SET Product_Name = @name, Quantity_Available = @qty WHERE Product_ID = @id";
            return _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", p.Product_Name),
                new MySqlParameter("@qty", p.Quantity_Available),
                new MySqlParameter("@id", p.Product_ID)) > 0;
        }
        #endregion

        #region Delete Product
        public override bool Delete(int productId)
        {
            string query = "DELETE FROM product WHERE Product_ID = @id";
            return _db.ExecuteNonQuery(query, new MySqlParameter("@id", productId)) > 0;
        }
        #endregion
    }

}
