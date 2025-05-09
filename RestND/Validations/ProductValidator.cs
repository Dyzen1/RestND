using RestND.Data;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Validations
{
    internal class ProductValidator
    {
        private readonly Product _product;
        private readonly ProductService _productService;

        public ProductValidator(Product product)
        {
            _product = product;
            _productService = new ProductService();
        }

        public bool ValidateProductName(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(_product.Product_Name))
            {
                errorMessage = "Product name cannot be empty!";
                return false;
            }

            List<Product> products = _productService.GetAll();

            if (products.Any(p => p.Product_Name == _product.Product_Name))
            {
                errorMessage = "Product with this name already exists!";
                return false;
            }

            return true;
        }

        public bool ValidateQuantityAvailable(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_product.Quantity_Available < 0)
            {
                errorMessage = "Quantity must be 0 or greater!";
                return false;
            }

            return true;
        }

    }
}
