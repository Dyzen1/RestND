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
                                match.Tolerance = product.Tolerance;
                                match.Quantity_Available = product.Quantity_Available;
                                match.Product_ID = product.Product_ID;
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
            AddProductCommand.NotifyCanExecuteChanged();
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
            //validations:
            // 1+2. check id input
            if (!_validator.IsEmptyField(NewProductIdInput, out string err))
            {
                ProductErrorMessage = err;
                return;
            }
            if (!_validator.isSerialNumValid(NewProductIdInput, out string erro))
            {
                ProductErrorMessage = erro;
                return;
            }
            // 3. product id existance validation
            if (!_validator.CheckIfIdExists(NewProductIdInput, Products.ToList(), out string idErr))
            {
                ProductErrorMessage = idErr;
                return;
            }
            // 4+5. check name input
            if (!_validator.IsEmptyField(NewProductNameInput, out string nErr))
            {
                ProductErrorMessage = nErr;
                return;
            }
            if (!_validator.isNameValid(NewProductNameInput, out string nameE))
            {
                ProductErrorMessage = nameE;
                return;
            }
            // 6. product name existance validation
            if (!_validator.CheckIfNameExists(NewProductNameInput, Products.ToList(), out string nameErr))
            {
                ProductErrorMessage = nameErr;
                return;
            }
            // 7. check that quantity && tolerance are a number
            if (!int.TryParse(NewQuantityInput, out int parsedQuantity) || !int.TryParse(NewToleranceInput, out int parsedTolerance))
            {
                ProductErrorMessage = "Please enter a valid number";
                return;
            }
            // 8. is quantity positive validation
            if (!_validator.CheckPosNum(parsedQuantity, out string qErr))
            {
                ProductErrorMessage = qErr;
                return;
            }
            // 9. is tolerance positive validation
            if (!_validator.CheckPosNum(parsedTolerance, out string tErr))
            {
                ProductErrorMessage = tErr;
                return;
            }
            // 10. validation passed
            ProductErrorMessage = string.Empty;

            NewProduct.Product_ID = NewProductIdInput;
            NewProduct.Product_Name = NewProductNameInput;
            NewProduct.Tolerance = parsedTolerance;
            NewProduct.Quantity_Available = parsedQuantity;

            bool success = _productService.Add(NewProduct);

            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", NewProduct, "add");
                NewProduct = new Inventory();
                //LoadProducts();
            }
        }

        #endregion

        #region Delete Product

        [RelayCommand]
        private async Task DeleteProductAsync()
        {

            if (SelectedProduct == null)
            {
                ProductErrorMessage = "Please select a product to delete.";
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
            // 1+2. check id input
            if (!_validator.IsEmptyField(SelectedProduct.Product_ID, out string err))
            {
                ProductErrorMessage = err;
                return;
            }
            if (!_validator.isSerialNumValid(SelectedProduct.Product_ID, out string erro))
            {
                ProductErrorMessage = erro;
                return;
            }
            // 3+4. check name input
            if (!_validator.IsEmptyField(SelectedProduct.Product_Name, out string nErr))
            {
                ProductErrorMessage = nErr;
                return;
            }
            if (!_validator.isNameValid(SelectedProduct.Product_Name, out string nameE))
            {
                ProductErrorMessage = nameE;
                return;
            }
            // 5. is quantity positive validation
            if (!_validator.CheckPosNum(SelectedProduct.Quantity_Available, out string qErr))
            {
                ProductErrorMessage = qErr;
                return;
            }
            // 6. is tolerance positive validation
            if (!_validator.CheckPosNumDouble(SelectedProduct.Tolerance, out string tErr))
            {
                ProductErrorMessage = tErr;
                return;
            }

            // 7. validation passed
            ProductErrorMessage = string.Empty;
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
