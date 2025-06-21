using RestND.Data;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestND.Validations
{
    public  class InventoryValidator
    {
        private readonly ProductService _productService;
        public  bool CheckIfNull(Inventory product, out string err)
        {
            err = string.Empty;
            if (product == null)
            {
                err = "You must choose a product";
                return false;
            }
            return true;
        }
        public bool CheckIfIdExists(string productId, List<Inventory> productList, out string err)
        {
            err = string.Empty;

            var doesExist = productList.FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(p.Product_ID) &&
                p.Product_ID.Equals(productId, StringComparison.OrdinalIgnoreCase));

            if (doesExist != null)
            {
                err = "Product ID already exists.";
                return false;
            }

            return true;
        }

        public bool CheckIfNameExists(string productName, List<Inventory> productList, out string err)
        {
            err = string.Empty;
            var doesExist = productList.FirstOrDefault(p =>
                p.Product_Name.Equals(productName, StringComparison.OrdinalIgnoreCase));

            if (doesExist != null)
            {
                err = "Product name already exists";
                return false;
            }
            return true;
        }
        public  bool PositiveTolerance(double tolerance, out string err)
        {
            err = string.Empty;
            if (tolerance <= 0)
            {
                err = "Tolerance must be a positive number.";
                return false;
            }
            return true;
        }
        public bool PositiveQuantity(int quantity, out string err)
        {
            err = string.Empty;
            if (quantity < 0)
            {
                err = "Quantity must be a non-negative.";
                return false;
            }
            return true;
        }
        

    }
}
