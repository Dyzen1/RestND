using MySql.Data.MySqlClient;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;

namespace RestND.Data
{
    public class ProductService
    {
        // this is a variable that uses the DatabaseOperations class to connect to the database
        //read only means that this variable can only be set in the constructor
        private readonly DatabaseOperations _db;

        // Constructor that uses our helper class to connect to the database
        public ProductService()
        {
            
            _db = new DatabaseOperations("127.0.0.1", "restnd", "root", "D123456N!");
        }

 
        /// Loads all products from the database and returns them as a list.
        public List<Product> GetProducts()
        {
            var products = new List<Product>();
            string query = "SELECT * FROM product";

            // use executeReader to get the data from the database
            var rows = _db.ExecuteReader(query);

            // Convert each row from the database into a Product object
            foreach (var row in rows)
            {
                var product = new Product
                {
                    Product_ID = Convert.ToInt32(row["Product_ID"]),
                    Product_Name = row["Product_Name"].ToString(),
                    Quantity_Available = Convert.ToInt32(row["Quantity_Available"])
                };

                products.Add(product);
            }

            return products;
        }


        /// Deletes a product from the database by its ID. 
        public bool DeleteProduct(int productId)
        {
            string query = "DELETE FROM product WHERE Product_ID = @id";

            // use the ExecuteNonQuery method to delete the product
            // using MySqlParameter is to prevent sql attacks like someone could write that product name is "'1; DROP TABLE products;"
            int affectedRows = _db.ExecuteNonQuery(query, new MySqlParameter("@id", productId));
            // Check if any rows were affected (deleted) if nothing was deleted, affectedRows will be 0
            return affectedRows > 0;
        }

       
        /// Updates a product's name and quantity in the database.
        public bool UpdateProduct(Product p)
        {
            string query = "UPDATE product SET Product_Name = @name, Quantity_Available = @qty WHERE Product_ID = @id";

            int affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", p.Product_Name),
                new MySqlParameter("@qty", p.Quantity_Available),
                new MySqlParameter("@id", p.Product_ID)
            );

            return affectedRows > 0;
        }

       
        /// Adds a new product to the database.

        public bool AddProduct(Product p)
        {
            string query = "INSERT INTO product (Product_Name, Quantity_Available) VALUES (@name, @qty)";

            int affectedRows = _db.ExecuteNonQuery(query,
                new MySqlParameter("@name", p.Product_Name),
                new MySqlParameter("@qty", p.Quantity_Available)
            );

            return affectedRows > 0;
        }
    }
}
