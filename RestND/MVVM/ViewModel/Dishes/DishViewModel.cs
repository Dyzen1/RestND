using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using RestND.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestND.MVVM.ViewModel
{
    public partial class DishViewModel : ObservableObject
    {
        #region Services

        private readonly DishServices _dishService;
        private readonly DishTypeServices _dishTypeService;
        private readonly ProductService _productService;

        [ObservableProperty]
        private ObservableCollection<DishType> dishTypes = new();

        #endregion

        #region Observable Properties

        [ObservableProperty]
        private ObservableCollection<Dish> dishes = new();

        [ObservableProperty]
        private ObservableCollection<Inventory> availableProducts = new();

        [ObservableProperty]
        private ObservableCollection<ProductInDish> selectedProducts = new();

        [ObservableProperty]
        private Dish newDish = new();

        [ObservableProperty]
        private Dish selectedDish;

        [ObservableProperty]
        private ObservableCollection<AllergenNotes> allergenNotes = new();

        [ObservableProperty]
        private ObservableCollection<AllergenNotes> selectedAllergenNotes = new();

        [ObservableProperty]
        private Dictionary<string, List<string>> dishValidationErrors = new();

        [ObservableProperty]
        private Inventory selectedAvailableProduct;

        [ObservableProperty]
        private int productAmountUsage;

        partial void OnSelectedDishChanged(Dish value)
        {
            //DeleteDishCommand.NotifyCanExecuteChanged();
            //UpdateDishCommand.NotifyCanExecuteChanged();
            //UpdateDishOnlyCommand.NotifyCanExecuteChanged();
            //UpdateProductsOnlyCommand.NotifyCanExecuteChanged();
            //UpdateDishTypeOnlyCommand.NotifyCanExecuteChanged();
            //UpdateAllDishDataCommand.NotifyCanExecuteChanged();
        }

        #endregion

        #region Constructor

        public DishViewModel()
        {
            _dishTypeService = new DishTypeServices();
            _dishService = new DishServices();
            _productService = new ProductService();

            DishTypes = new ObservableCollection<DishType>(_dishTypeService.GetAll());
            LoadNotes();
            LoadDishes();
            LoadAvailableProducts();
        }

        #endregion

        #region Load Methods

        [RelayCommand]
        private void LoadDishes()
        {
            Dishes.Clear();
            var dbDishes = _dishService.GetAll();
            foreach (var dish in dbDishes)
                Dishes.Add(dish);
        }

        [RelayCommand]
        private void LoadAvailableProducts()
        {
            AvailableProducts.Clear();
            var dbProducts = _productService.GetAll();
            foreach (var product in dbProducts)
                AvailableProducts.Add(product);
        }

        [RelayCommand]
        private void LoadNotes()
        {
            AllergenNotes = new ObservableCollection<AllergenNotes>(
                Enum.GetValues(typeof(AllergenNotes)).Cast<AllergenNotes>());
        }

        #endregion

        #region Dish Creation

        [RelayCommand(CanExecute = nameof(CanAddDish))]
        private void AddDish()
        {
            NewDish.ProductUsage = new List<ProductInDish>(SelectedProducts);
            NewDish.Allergen_Notes = SelectedAllergenNotes.ToList();

            dishValidationErrors = DishValidator.ValidateFields(NewDish, Dishes.ToList());

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
        #endregion

        #region Can Execute Methods


        private bool CanModifyDish() => SelectedDish != null;

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

        #region Update Options

        [RelayCommand(CanExecute = nameof(CanUpdateDish))]
        private void UpdateDish()
        {
            if (SelectedDish != null && _dishService.Update(SelectedDish))
                LoadDishes();
        }



        #endregion
    }
}
