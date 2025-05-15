using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using System;
using RestND.Validations;
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
        private readonly DishTypeServices _dishTypeService;

        // Service for handling Product database operations (to load available products)
        private readonly ProductService _productService;

        [ObservableProperty]
        private ObservableCollection<DishType> dishTypes = new ();
        #endregion

        #region Observable Properties

        // List of dishes displayed in the UI
        [ObservableProperty]
        private ObservableCollection<Dish> dishes = new();

        // List of available products that can be added to a dish
        [ObservableProperty]
        private ObservableCollection<Inventory> availableProducts = new();

        // List of selected products (with amount) that will be used in the new dish
        [ObservableProperty]
        private ObservableCollection<ProductInDish> selectedProducts = new();

        // The dish that is currently being created
        [ObservableProperty]
        private Dish newDish = new();

        // The dish currently selected in the UI
        [ObservableProperty]
        private Dish selectedDish;

        [ObservableProperty]
        private ObservableCollection<AllergenNotes> allergenNotes = new();

        [ObservableProperty]
        private ObservableCollection<AllergenNotes> selectedAllergenNotes = new ();

        [ObservableProperty]
        private Dictionary<string, List<string>> dishValidationErrors = new();

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
            _dishTypeService = new DishTypeServices();
            _dishService = new DishServices();
            _productService = new ProductService();
            dishTypes =new ObservableCollection<DishType>(_dishTypeService.GetAll());
            LoadNotes();
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

        // Loads allergen notes from the database
        [RelayCommand]
        private void LoadNotes()
        {
            AllergenNotes = new ObservableCollection<AllergenNotes>(
                Enum.GetValues(typeof(AllergenNotes)).Cast<AllergenNotes>());
        }

        #endregion

        #region Product Selection for Dish

        // Product selected by user from the AvailableProducts list
        [ObservableProperty]
        private Inventory selectedAvailableProduct;

        // Amount of product usage entered by the user (in grams or ml)
        [ObservableProperty]
        private int productAmountUsage;

        // Adds the selected product and entered amount to the SelectedProducts list
        [RelayCommand(CanExecute = nameof(CanAddDish))]
        private void AddDish()
        {
            NewDish.ProductUsage = new List<ProductInDish>(SelectedProducts);
            NewDish.Allergen_Notes = SelectedAllergenNotes.ToList();

            // Run validation
            dishValidationErrors = DishValidator.ValidateFields(NewDish, Dishes.ToList());

            // If any errors, return and display them
            if (dishValidationErrors.Any())
                return;

            bool success = _dishService.Add(NewDish);

            if (success)
            {
                LoadDishes();
                NewDish = new Dish();
                SelectedProducts.Clear();
                dishValidationErrors.Clear();
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

        // Helpers method: checks if a dish is selected (used for button enabling)
        private bool CanModifyDish()
        {
            return SelectedDish != null;
        }

        private bool CanAddDish()
        {
            NewDish.ProductUsage = new List<ProductInDish>(SelectedProducts);
            NewDish.Allergen_Notes = SelectedAllergenNotes.ToList();

            var errors = DishValidator.ValidateFields(NewDish, Dishes.ToList());
            return !errors.Any();
        }

        private bool CanUpdateDish()
        {
            if (SelectedDish == null)
                return false;

            var errors = DishValidator.ValidateFields(SelectedDish, Dishes.Where(d => d.Dish_ID != SelectedDish.Dish_ID).ToList());
            return !errors.Any();
        }


        #endregion

        #region Update Dish (optional)

        // Updates the selected dish information (without updating product usage)
        [RelayCommand(CanExecute = nameof(CanUpdateDish))]
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
