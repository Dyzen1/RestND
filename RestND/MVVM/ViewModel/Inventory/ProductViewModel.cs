using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
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

        #region Selected Product Change Hook

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
            bool success = _productService.Add(NewProduct);
            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", NewProduct, "add");
                NewProduct = new Inventory(); // reset
            }
        }

        #endregion

        #region Update Product

        [RelayCommand]
        private async Task UpdateProduct()
        {
            if (!CanModifyProduct()) return;

            bool success = _productService.Update(SelectedProduct);
            if (success)
            {
                await _hub.SendAsync("NotifyInventoryUpdate", SelectedProduct, "update");
            }
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
