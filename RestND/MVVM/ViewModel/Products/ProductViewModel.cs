using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
using System.Collections.ObjectModel;

namespace RestND.MVVM.ViewModel
{
    public partial class ProductViewModel : ObservableObject
    {
        #region Service
        // Service that handles database logic for products
        private readonly ProductService _productService;
        #endregion

        #region Observable Properties

        // A list of products bound to the UI
        [ObservableProperty]
        private ObservableCollection<Product> products = new();

        // The product selected in the UI
        [ObservableProperty]
        private Product selectedProduct;

        partial void OnSelectedProductChanged(Product value)
        {
            DeleteProductCommand.NotifyCanExecuteChanged();
            UpdateProductCommand.NotifyCanExecuteChanged();
        }

        // New product to be added — bound to the UI
        [ObservableProperty]
        private Product newProduct = new();

        #endregion

        #region Constructor

        public ProductViewModel()
        {
            _productService = new ProductService(); // Create the service
            LoadProducts(); // Load all products on startup
        }

        #endregion

        #region Load Products

        // Loads products from the database and displays them in the UI
        [RelayCommand]
        private void LoadProducts()
        {
            Products.Clear(); // Clear existing items

            var dbProducts = _productService.GetProducts(); // Fetch from DB

            foreach (var product in dbProducts)
            {
                Products.Add(product); // Add each one to the list
            }
        }

        #endregion

        #region Add Product

        // Adds a new product to the database and refreshes the product list
        [RelayCommand]
        private void AddProduct()
        {
            // Check if the new product has valid data
            if (!string.IsNullOrWhiteSpace(newProduct.Product_Name) && newProduct.Quantity_Available >= 0)
            {
                bool success = _productService.AddProduct(newProduct);

                if (success)
                {
                    LoadProducts(); // Refresh the list
                    newProduct = new Product(); // Clear the input fields
                }
            }
        }

        #endregion

        #region Delete Product

        // Deletes the selected product from the DB and removes it from the list
        // canExecute is used to check if a product is selected the method CanModifyProduct will return true or false
        // the nameof operator is used to get the name of the method as a string , so if we rename the method, it updates automatically
        [RelayCommand(CanExecute = nameof(CanModifyProduct))]
        private void DeleteProduct()
        {
            // Always double-check that a product is selected
            if (SelectedProduct != null)
            {
                bool success = _productService.DeleteProduct(SelectedProduct.Product_ID);

                if (success)
                {
                    Products.Remove(SelectedProduct); // Remove from UI
                }
            }
        }

        #endregion

        #region Update Product

        // Updates the selected product's info in the DB
        [RelayCommand(CanExecute = nameof(CanModifyProduct))]
        private void UpdateProduct()
        {
            if (SelectedProduct != null)
            {
                bool success = _productService.UpdateProduct(SelectedProduct);

                if (success)
                {
                    // Replace it in the UI list
                    var index = Products.IndexOf(SelectedProduct);
                    if (index >= 0)
                        Products[index] = SelectedProduct;
                }
            }
        }

        #endregion

        #region CanExecute Helpers

        // Checks if a product is selected (used to enable/disable buttons)
        private bool CanModifyProduct()
        {
            return SelectedProduct != null;
        }

        #endregion
    }
}
