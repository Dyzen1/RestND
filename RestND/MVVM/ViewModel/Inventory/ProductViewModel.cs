using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.Validations;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestND.MVVM.ViewModel
{
    public partial class ProductViewModel : ObservableObject
    {
        #region Services

        private readonly ProductService _productService;
        private readonly InventoryValidator _validator = new();
        private readonly HubConnection _hub;

        #endregion

        #region Observable Properties

        [ObservableProperty] private ObservableCollection<Inventory> products = new();
        [ObservableProperty] private Inventory selectedProduct;
        [ObservableProperty] private Inventory newProduct = new();

        [ObservableProperty] private string productIdError;
        [ObservableProperty] private string productNameError;
        [ObservableProperty] private string quantityError;
        [ObservableProperty] private string toleranceError;

        #endregion

        #region Constructor

        public ProductViewModel()
        {
            _productService = new ProductService();
            _hub = App.InventoryHub;

            _hub.On<Inventory, string>("ReceiveInventoryUpdate", (product, action) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var match = Products.FirstOrDefault(p => p.Product_ID == product.Product_ID);

                    switch (action)
                    {
                        case "add":
                            if (match == null)
                                Products.Add(product);
                            break;
                        case "update":
                            if (match != null)
                            {
                                match.Product_Name = product.Product_Name;
                                match.Quantity_Available = product.Quantity_Available;
                                match.Tolerance = product.Tolerance;
                            }
                            break;
                        case "delete":
                            if (match != null)
                                Products.Remove(match);
                            break;
                    }
                });
            });

            LoadProducts();
        }

        #endregion

        #region On Change

        partial void OnSelectedProductChanged(Inventory value)
        {
            UpdateProductCommand.NotifyCanExecuteChanged();
            DeleteProductCommand.NotifyCanExecuteChanged();
        }

        #endregion

        #region Load Products

        [RelayCommand]
        private void LoadProducts()
        {
            Products.Clear();
            var dbProducts = _productService.GetAll();
            foreach (var product in dbProducts)
            {
                Products.Add(product);
            }
        }

        #endregion

        #region Add Product

        [RelayCommand]
        private async Task AddProduct()
        {
            ClearErrors();

            if (string.IsNullOrWhiteSpace(NewProduct.Product_ID))
            {
                ProductIdError = "Product ID is required.";
                return;
            }

            if (!_validator.CheckIfIdExists(NewProduct.Product_ID, Products.ToList(), out string idErr))
            {
                ProductIdError = idErr;
                return;
            }

            if (string.IsNullOrWhiteSpace(NewProduct.Product_Name))
            {
                ProductNameError = "Product name is required.";
                return;
            }

            if (!_validator.CheckIfNameExists(NewProduct.Product_Name, Products.ToList(), out string nameErr))
            {
                ProductNameError = nameErr;
                return;
            }

            if (!_validator.PositiveQuantity(NewProduct.Quantity_Available, out string qtyErr))
            {
                QuantityError = qtyErr;
                return;
            }

            if (!_validator.PositiveTolerance(NewProduct.Tolerance, out string tolErr))
            {
                ToleranceError = tolErr;
                return;
            }

            bool success = _productService.Add(NewProduct);

            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", NewProduct, "add");
                NewProduct = new Inventory();
                ClearErrors();
                LoadProducts();
            }
        }

        #endregion

        #region Delete Product

        [RelayCommand]
        private async Task DeleteProductAsync()
        {
            ClearErrors();

            if (SelectedProduct == null)
            {
                ProductIdError = "Please select a product to delete.";
                return;
            }

            bool success = _productService.Delete(SelectedProduct);
            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", SelectedProduct, "delete");
                LoadProducts();
            }
        }

        #endregion

        #region Update Product

        [RelayCommand]
        private async Task UpdateProduct()
        {
            ClearErrors();

            if (SelectedProduct == null)
            {
                ProductIdError = "Please select a product to update.";
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedProduct.Product_Name))
            {
                ProductNameError = "Product name is required.";
                return;
            }

            if (!_validator.PositiveQuantity(SelectedProduct.Quantity_Available, out string qtyErr))
            {
                QuantityError = qtyErr;
                return;
            }

            if (!_validator.PositiveTolerance(SelectedProduct.Tolerance, out string tolErr))
            {
                ToleranceError = tolErr;
                return;
            }

            bool success = _productService.Update(SelectedProduct);

            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", SelectedProduct, "update");
                LoadProducts();
            }
        }

        #endregion

        #region Helpers

        private void ClearErrors()
        {
            ProductIdError = ProductNameError = QuantityError = ToleranceError = string.Empty;
        }

        #endregion
    }
}
