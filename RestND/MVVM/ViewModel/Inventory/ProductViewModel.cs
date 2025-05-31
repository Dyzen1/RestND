using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.Validations;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace RestND.MVVM.ViewModel
{
    public partial class ProductViewModel : ObservableObject
    {
        #region Fields & Services

        private readonly ProductService _productService;

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
        private void AddProduct()
        {


            bool success = _productService.Add(NewProduct);
            if (success)
            {
                LoadProducts();
                NewProduct = new Inventory(); // reset
            }
        }

        #endregion

        #region Update Product

        [RelayCommand]
        private void UpdateProduct()
        {
            
;

            bool success = _productService.Update(SelectedProduct);
            if (success)
            {
                var index = Products.IndexOf(SelectedProduct);
                if (index >= 0)
                    Products[index] = SelectedProduct;
            }
        }

        #endregion

        #region Delete Product

        [RelayCommand]
        private void DeleteProduct()
        {
            

            bool success = _productService.Delete(SelectedProduct);
            if (success)
            {
                Products.Remove(SelectedProduct);
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
