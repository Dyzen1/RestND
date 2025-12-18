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

        [ObservableProperty] private string productErrorMessage;
        [ObservableProperty] private string newProductIdInput;
        [ObservableProperty] private string newProductNameInput;
        [ObservableProperty] private string newQuantityInput;
        [ObservableProperty] private string newToleranceInput;
        #endregion

        #region Constructor
        public ProductViewModel()
        {
            _productService = new ProductService();
            _hub = App.InventoryHub;
            InitializeSR();
            LoadProducts();
        }
        #endregion

        #region SignalR Method
        private void InitializeSR()
        {
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
        }

        #endregion

        //#region On Change
        //partial void OnSelectedProductChanged(Inventory value)
        //{
        //    UpdateProductCommand.NotifyCanExecuteChanged();
        //    DeleteProductCommand.NotifyCanExecuteChanged();
        //    AddProductCommand.NotifyCanExecuteChanged();
        //}
        //#endregion

        #region Load Products
        [RelayCommand]
        private void LoadProducts()
        {
            Products.Clear();
            var dbProducts = _productService.GetAll();
            foreach (var product in dbProducts)
            {
                if(product.Quantity_Available <= product.Tolerance)
                {
                    product.In_Stock = false;
                    _productService.UpdateInStock(product.Product_ID, false);
                }
                Products.Add(product);
            }
        }
        #endregion

        #region Add Product
        [RelayCommand]
        private async Task AddProduct()
        {
            // Use the new "Dish-style" validator
            if (!_validator.ValidateForAdd(
                    NewProductIdInput,
                    NewProductNameInput,
                    NewQuantityInput,
                    NewToleranceInput,
                    Products,
                    out int parsedQuantity,
                    out double parsedTolerance,
                    out string err))
            {
                ProductErrorMessage = err;
                return;
            }

            ProductErrorMessage = string.Empty;

            NewProduct.Product_ID = NewProductIdInput;
            NewProduct.Product_Name = NewProductNameInput;
            NewProduct.Quantity_Available = parsedQuantity;
            NewProduct.Tolerance = parsedTolerance;

            bool success = _productService.Add(NewProduct);

            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", NewProduct, "add");
                NewProduct = new Inventory();

            }
        }

        #endregion

        #region Delete Product
        [RelayCommand]
        private async Task DeleteProductAsync()
        {
            // checking if a product had been selected
            if (SelectedProduct == null)
            {
                ProductErrorMessage = "Please select a product to delete.";
                return;
            }
            // if a product had been selected, activate the Delete function
            bool success = _productService.Delete(SelectedProduct);
            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", SelectedProduct, "delete");

                await App.DishHub.SendAsync("NotifyDishUpdate", null, "product-deleted");
                LoadProducts();
            }
        }

        #endregion

        #region Update Product
        [RelayCommand]
        private async Task UpdateProduct()
        {
            // checking if a product had been selected
            if (SelectedProduct == null)
            {
                ProductErrorMessage = "Please select a product to update.";
                return;
            }

            if (!_validator.ValidateForUpdate(SelectedProduct, Products, out string err))
            {
                ProductErrorMessage = err;
                return;
            }

            ProductErrorMessage = string.Empty;

            // If you use this flag in DB:
            SelectedProduct.Is_Active = true;

            bool success = _productService.Update(SelectedProduct);
            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", SelectedProduct, "update");
                LoadProducts();
            }
        }
        #endregion

    }
}
