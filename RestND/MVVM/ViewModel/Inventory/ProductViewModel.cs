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

        #region On Change
        partial void OnSelectedProductChanged(Inventory value)
        {
            DeleteProductCommand.NotifyCanExecuteChanged();
            UpdateProductCommand.NotifyCanExecuteChanged();
        }

        #endregion

        #region Load Products

        [RelayCommand]
        private void LoadProducts()
        {
            Products.Clear();

            var dbProducts = _productService.GetAll(); // updated GetProducts -> GetAll

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

        #region Delete Product

        [RelayCommand(CanExecute = nameof(CanModifyProduct))]
        private void DeleteProduct()
        {
            if (SelectedProduct != null)
            {
                bool success = _productService.Delete(SelectedProduct.Product_ID); //  updated DeleteProduct -> Delete

                if (success)
                {
                    Products.Remove(SelectedProduct);
                }
            }
        }

        #endregion

        #region Update Product

        [RelayCommand(CanExecute = nameof(CanModifyProduct))]
        private void UpdateProduct()
        {
            if (SelectedProduct != null)
            {
                bool success = _productService.Update(SelectedProduct); //  updated UpdateProduct -> Update

                if (success)
                {
                    var index = Products.IndexOf(SelectedProduct);
                    if (index >= 0)
                        Products[index] = SelectedProduct;
                }
            }
        }

        #endregion

        #region CanExecute Helpers

        private bool CanModifyProduct()
        {
            return SelectedProduct != null;
        }

        #endregion
    }
}
