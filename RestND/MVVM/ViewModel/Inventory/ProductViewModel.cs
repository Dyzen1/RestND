using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Windows;

namespace RestND.MVVM.ViewModel
{
    public partial class ProductViewModel : ObservableObject
    {
        #region Services

        private readonly ProductService _productService;
        private readonly HubConnection _hub;

        #endregion

        #region Observable Properties

        private readonly HubConnection _hub;

        [ObservableProperty]
        public ObservableCollection<Inventory> products = new();

        [ObservableProperty]
        public Inventory selectedProduct;

        [ObservableProperty]
        public Inventory newProduct = new();

        #endregion

        #region Constructor

        public ProductViewModel()
        {
            _productService = new ProductService();
            _hub = App.HubConnection;
            _hub.On<Inventory>("ReceiveInventoryUpdate", (updatedProduct) =>
            {
                var match = Products.FirstOrDefault(p => p.Product_ID == updatedProduct.Product_ID);
                if (match != null)
                {
                    match.Product_Name = updatedProduct.Product_Name;
                    match.Quantity_Available = updatedProduct.Quantity_Available;
                    match.Tolerance = updatedProduct.Tolerance;
                }
                else
                {
                    Products.Add(updatedProduct);
                }
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
=======
        #region Selected Product Change Hook

>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
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
        private async void AddProduct()
        {
<<<<<<< HEAD
            bool success = _productService.Add(NewProduct); 

            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", NewProduct);
                LoadProducts();
                NewProduct = new Inventory(); //  call the setter so UI refreshes
            }
        }

        #endregion

        #region Delete Product

        [RelayCommand]
        private void DeleteProduct()
        {
            if(CanModifyProduct())
            {
                bool success = _productService.Delete(SelectedProduct); 

                if (success)
                {
                    Products.Remove(SelectedProduct);
                }
=======
            bool success = _productService.Add(NewProduct);
            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", NewProduct, "add");
                NewProduct = new Inventory(); // reset
>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
            }
        }

        #endregion

        #region Update Product

        [RelayCommand]
<<<<<<< HEAD
        private async void UpdateProduct()
=======
        private async Task UpdateProduct()
>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
        {
            if (!CanModifyProduct()) return;

            bool success = _productService.Update(SelectedProduct);
            if (success)
            {
<<<<<<< HEAD
                await _hub.SendAsync("NotifyInventoryUpdate", SelectedProduct);
                var index = Products.IndexOf(SelectedProduct);
                if (index >= 0)
                    Products[index] = SelectedProduct;
            }

=======
                await _hub.SendAsync("NotifyInventoryUpdate", SelectedProduct, "update");
            }
>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
        }

        #endregion

        #region Delete Product

        [RelayCommand]
        private async void DeleteProduct()
        {
            if (!CanModifyProduct()) return;

            bool success = _productService.Delete(SelectedProduct);
            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", SelectedProduct, "delete");
            }
        }

        #endregion

        #region Helpers

        private bool CanModifyProduct()
        {
            return SelectedProduct != null;
        }

        #endregion
    }
}
