using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        private void AddProduct()
        {
            if (!string.IsNullOrWhiteSpace(NewProduct.Product_Name) && NewProduct.Quantity_Available >= 0)
            {
                bool success = _productService.Add(NewProduct); //  updated AddProduct -> Add

                if (success)
                {
                    LoadProducts();
                    NewProduct = new Inventory(); //  call the setter so UI refreshes
                }
            }
        }

        #endregion

        #region Update Product

        [RelayCommand]
        private async Task DeleteProductAsync()
        {
            if (CanModifyProduct())
            {
                bool success = _productService.Delete(SelectedProduct);
                if (success)
                {
                    await _hub.SendAsync("NotifyInventoryUpdate", SelectedProduct, "update");
                }
            }
        }

        #endregion

        #region Delete Product

        [RelayCommand]
        private void UpdateProduct()
        {
            if(CanModifyProduct())
            {
                bool success = _productService.Update(SelectedProduct); 

                if (success)
                {
                    var index = Products.IndexOf(SelectedProduct);
                    if (index >= 0)
                        Products[index] = SelectedProduct;
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
