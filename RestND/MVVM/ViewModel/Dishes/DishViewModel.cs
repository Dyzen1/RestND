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

        #region On Change

        partial void OnSelectedDishChanged(Dish value)
        {
            UpdateDishCommand.NotifyCanExecuteChanged();
            DeleteDishCommand.NotifyCanExecuteChanged();
            AddDishCommand.NotifyCanExecuteChanged();
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
            //AllergenNotes = new ObservableCollection<AllergenNotes>(
            //    Enum.GetValues(typeof(AllergenNotes)).Cast<AllergenNotes>());
            AllergenNotes = new ObservableCollection<AllergenNotes>(
                    Enum.GetValues(typeof(AllergenNotes)).Cast<AllergenNotes>()
  );

        }

        #endregion

        #region Helpers
        private List<ProductInDish> CloneSelectedProducts() =>
            SelectedProducts.Select(p => new ProductInDish
            {
                Product_ID = p.Product_ID,
                Amount_Usage = p.Amount_Usage
            }).ToList();

        #endregion

        #region Add Dish

        [RelayCommand(CanExecute = nameof(CanAddDish))]
        private void AddDish()
        {
            NewDish.ProductUsage = CloneSelectedProducts();
            NewDish.Allergen_Notes = SelectedAllergenNotes.ToList();

            bool success = _dishService.Add(NewDish);

            if (success)
            {
                LoadDishes();
                NewDish = new Dish();
                SelectedProducts.Clear();
            }
        }

        #endregion

        #region Add Product To Dish

        [RelayCommand]
        public void AddProductToDish()
        {
            if (SelectedAvailableProduct != null && ProductAmountUsage > 0)
            {
                var product = new ProductInDish
                {
                    Product_ID = SelectedAvailableProduct.Product_ID,
                    Amount_Usage = ProductAmountUsage
                };

                if (!SelectedProducts.Any(p => p.Product_ID == product.Product_ID))
                {
                    SelectedProducts.Add(product);
                }

                SelectedAvailableProduct = null;
                ProductAmountUsage = 0;
            }
        }

        #endregion

        #region Delete Dish

        [RelayCommand(CanExecute = nameof(CanModifyDish))]
        private void DeleteDish()
        {
            if (SelectedDish != null)
            {
                bool success = _dishService.Delete(SelectedDish);
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
            NewDish.ProductUsage = CloneSelectedProducts();
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

        #region Update Dish

        [RelayCommand(CanExecute = nameof(CanUpdateDish))]
        private void UpdateDish()
        {
            if (SelectedDish != null && _dishService.Update(SelectedDish))
                LoadDishes();
        }

        #endregion
    }
}
