using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model;
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
        private readonly HubConnection _hub;

        #endregion

        #region Observable Properties

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
            if (!string.IsNullOrWhiteSpace(NewProduct.Product_Name) && NewProduct.Quantity_Available >= 0)
            {
                bool success = _productService.Add(NewProduct);

                if (success)
                {
                    await _hub.SendAsync("NotifyInventoryUpdate", NewProduct, "add");
                    NewProduct = new Inventory(); 
                    LoadProducts(); // Refresh the list
                }
            }
        }

        #endregion

        #region Delete Product

        [RelayCommand]
        private async Task DeleteProductAsync()
        {
            if (CanModifyProduct())
            {
                bool success = _productService.Delete(SelectedProduct);
                if (success)
                {
                    await _hub.SendAsync("NotifyInventoryUpdate", SelectedProduct, "delete");
                    LoadProducts(); // Refresh the list
                }
            }
        }

        #endregion

        #region Update Product

        [RelayCommand]
        private async Task UpdateProduct()
        {
            if(CanModifyProduct())
            {
                bool success = _productService.Update(SelectedProduct); 

                if (success)
                {
                    await _hub.SendAsync("NotifyInventoryUpdate", SelectedProduct, "update");
                    LoadProducts(); // Refresh the list
                }
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
