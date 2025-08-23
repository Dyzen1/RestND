using RestND.Data;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestND.Validations
{
    public  class InventoryValidator : GeneralValidations
    {
        private readonly ProductService _productService = new();
        public  bool CheckIfNull(Inventory product, out string err)
        {
            err = string.Empty;
            if (product == null)
            {
                err = "You must choose a product to update";
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

        public bool isSerialNumValid(string num, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(num))
            {
                errorMessage = "Please insert a serial number";
                return false;
            }
            string pattern = @"^\S{2,50}$";
            if (!Regex.IsMatch(num, pattern))
            {
                errorMessage = "Invalid serial number!";
                return false;
            }
            return true;
        }
    }
}
