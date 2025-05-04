using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestND.MVVM.ViewModel
{
    // ViewModel for managing Dishes (used in the UI)
    public partial class DishViewModel : ObservableObject
    {
        #region Services

        // Service for handling Dish database operations
        private readonly DishServices _dishService;

        // Service for handling Product database operations (to load available products)
        private readonly ProductService _productService;

        #endregion

        #region Observable Properties

        // List of dishes displayed in the UI
        [ObservableProperty]
        private ObservableCollection<Dish> dishes = new();

        // List of available products that can be added to a dish
        [ObservableProperty]
        private ObservableCollection<Product> availableProducts = new();

        // List of selected products (with amount) that will be used in the new dish
        [ObservableProperty]
        private ObservableCollection<ProductUsageInDish> selectedProducts = new();

        // The dish that is currently being created
        [ObservableProperty]
        private Dish newDish = new();

        // The dish currently selected in the UI
        [ObservableProperty]
        private Dish selectedDish;

        // Called automatically when SelectedDish changes
        partial void OnSelectedDishChanged(Dish value)
        {
            DeleteDishCommand.NotifyCanExecuteChanged();
            UpdateDishCommand.NotifyCanExecuteChanged();
        }

        #endregion

        #region Constructor

        // Initializes services and loads data when the ViewModel is created
        public DishViewModel()
        {
            _dishService = new DishServices();
            _productService = new ProductService();

            LoadDishes();
            LoadAvailableProducts();
        }

        #endregion

        #region Load Methods

        // Loads all dishes from the database
        [RelayCommand]
        private void LoadDishes()
        {
            Dishes.Clear();
            var dbDishes = _dishService.GetAll();
            foreach (var dish in dbDishes)
                Dishes.Add(dish);
        }

        // Loads all available products from the database
        [RelayCommand]
        private void LoadAvailableProducts()
        {
            AvailableProducts.Clear();
            var dbProducts = _productService.GetAll();
            foreach (var product in dbProducts)
                AvailableProducts.Add(product);
        }

        #endregion

        #region Product Selection for Dish

        // Product selected by user from the AvailableProducts list
        [ObservableProperty]
        private Product selectedAvailableProduct;

        // Amount of product usage entered by the user (in grams or ml)
        [ObservableProperty]
        private int productAmountUsage;

        // Adds the selected product and entered amount to the SelectedProducts list
        [RelayCommand]
        private void AddProductToDish()
        {
            if (SelectedAvailableProduct == null || ProductAmountUsage <= 0)
                return;

            var usage = new ProductUsageInDish
            {
                Product_ID = SelectedAvailableProduct.Product_ID,
                Product_Name = SelectedAvailableProduct.Product_Name,
                Amount_Usage = ProductAmountUsage
            };

            SelectedProducts.Add(usage);

            // Reset selection for next product
            SelectedAvailableProduct = null;
            ProductAmountUsage = 0;
        }

        #endregion

        #region Add Dish

        // Adds the new dish (and its selected products) to the database
        [RelayCommand]
        private void AddDish()
        {
            if (string.IsNullOrWhiteSpace(NewDish.Dish_Name) || NewDish.Dish_Price <= 0)
                return;

            // Attach the selected products to the NewDish
            NewDish.ProductUsage = new List<ProductUsageInDish>(SelectedProducts);

            bool success = _dishService.Add(NewDish);

            if (success)
            {
                LoadDishes();
                NewDish = new Dish();
                SelectedProducts.Clear();
            }
        }

        #endregion

        #region Delete Dish

        // Deletes the selected dish
        [RelayCommand(CanExecute = nameof(CanModifyDish))]
        private void DeleteDish()
        {
            if (SelectedDish != null)
            {
                bool success = _dishService.Delete(SelectedDish.Dish_ID);

                if (success)
                {
                    Dishes.Remove(SelectedDish);
                }
            }
        }

        // Helper method: checks if a dish is selected (used for button enabling)
        private bool CanModifyDish()
        {
            return SelectedDish != null;
        }

        #endregion

        #region Update Dish (optional)

        // Updates the selected dish information (without updating product usage)
        [RelayCommand(CanExecute = nameof(CanModifyDish))]
        private void UpdateDish()
        {
            if (SelectedDish != null)
            {
                bool success = _dishService.Update(SelectedDish);

                if (success)
                {
                    LoadDishes();
                }
            }
        }

        #endregion
    }
}
