using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using RestND.MVVM.ViewModel.Dishes;
using RestND.Validations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestND.MVVM.ViewModel
{
    public partial class EditDishViewModel : ObservableObject
    {
        #region Services
        private readonly DishValidator _dishValidator = new();
        private readonly DishServices _dishService = new();
        private readonly DishTypeServices _dishTypeService = new();
        private readonly ProductService _productService = new();
        private readonly ProductInDishService _productInDishService= new();
        private readonly HubConnection _hub;
        #endregion

        #region Observable properties
        [ObservableProperty] private ObservableCollection<SelectableProduct> productSelections = new();

        [ObservableProperty] private Dish selectedDish;
        [ObservableProperty] private ObservableCollection<DishType> dishTypes = new();
        [ObservableProperty] private ObservableCollection<SelectableItem<string>> allergenOptions = new();

        [ObservableProperty] private ObservableCollection<Inventory> availableProducts = new();
        private readonly List<string> _allPossibleAllergens = new()
        {
            "Contains Gluten/Wheat",
            "Contains Peanuts",
            "Contains Tree Nuts",
            "Contains Milk Or Dairy Ingredients",
            "Contains Eggs Or Egg-Derived Ingredients",
            "Contains Soy Or Soy-Derived Ingredients",
            "Contains Fish",
            "Contains Shellfish",
            "Contains Sesame",
            "Contains Mustard",
            "Contains Celery",
            "Contains Sulfites"
        };

        [ObservableProperty] private string dishErrorMessage;
        #endregion

        #region Constructor
        public EditDishViewModel(Dish dishToEdit)
        {
            _hub = App.DishHub;
            _productService = new ProductService();

            SelectedDish = new Dish
            {
                Dish_ID = dishToEdit.Dish_ID,
                Dish_Name = dishToEdit.Dish_Name,
                Dish_Price = dishToEdit.Dish_Price,
                Dish_Type = dishToEdit.Dish_Type,
                Allergen_Notes = dishToEdit.Allergen_Notes
            };

            SelectedDish.ProductUsage = _productInDishService.GetProductsInDish(SelectedDish.Dish_ID).ToList();

            // saving the former user's selection of dish type.
            DishTypes = new ObservableCollection<DishType>(_dishTypeService.GetAll());
            if (SelectedDish.Dish_Type != null)
            {
                var matchingType = DishTypes.FirstOrDefault(dt => dt.DishType_Name == SelectedDish.Dish_Type.DishType_Name);
                if (matchingType != null)
                    SelectedDish.Dish_Type = matchingType;
            }

            BuildAllergenOptions();
            AvailableProducts = new ObservableCollection<Inventory>(_productService.GetAll());
            GetProductSelections();

        }
        #endregion
        
        #region On change
        partial void OnSelectedDishChanged(Dish value)
        {
            UpdateDishCommand.NotifyCanExecuteChanged();
        }
        #endregion

        #region Relay commands

        [RelayCommand]
        private async Task UpdateDish()
        {
            // Build ProductUsage from the grid rows the user checked + filled
            var chosen = ProductSelections
                .Where(x => x.IsSelected)
                .Select(x => new ProductInDish
                {
                    Product_ID = x.Product_ID,
                    Product_Name = x.Product_Name,
                    Amount_Usage = x.AmountUsage,
                    Dish_ID = SelectedDish.Dish_ID // keep 0 if DB assigns
                })
                .ToList();

            //validations:
            if (AllergenOptions.Count == 0)
            {
                DishErrorMessage = "Please select at least one allergen note";
                return;
            }
            // 1. check if inputs are empty
            if (SelectedDish.Dish_Price == null || SelectedDish.Dish_Type == null ||
                chosen == null || string.IsNullOrEmpty(SelectedDish.Dish_Name) || 
                string.IsNullOrWhiteSpace(SelectedDish.Dish_Name))
            {
                DishErrorMessage = "All fields must be populated";
                return;
            }
            // 2. check if product usage is empty
            if (chosen.Count == 0)
            {
                DishErrorMessage = "Please select at least one product with a positive amount";
                return;
            }
            // 3. is price positive validation
            if (!_dishValidator.CheckPosNum(SelectedDish.Dish_Price, out string priceErr))
            {
                DishErrorMessage = priceErr;
                return;
            }
            // 4. validation passed
            DishErrorMessage = string.Empty;

            SelectedDish.ProductUsage = chosen;
            SelectedDish.Allergen_Notes = string.Join(", ",
            AllergenOptions.Where(a => a.IsSelected).Select(a => a.Value));// Allergens from checkboxes
            SelectedDish.Is_Active = true; 

            bool success = _dishService.Update(SelectedDish);
            if (success)
            {
                await _hub.SendAsync("NotifyDishUpdate", SelectedDish, "update");
                MessageBox.Show("Dish updated successfully.", "Success");
            }
        }

        public void BuildAllergenOptions()
        {
            AllergenOptions.Clear();

            var selectedNotes = (SelectedDish.Allergen_Notes ?? "")
                .Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            foreach (var allergen in _allPossibleAllergens)
            {
                var item = new SelectableItem<string>(allergen)
                {
                    IsSelected = selectedNotes.Contains(allergen)
                };

                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(SelectableItem<string>.IsSelected))
                    {
                        SelectedDish.Allergen_Notes = string.Join(", ",
                            AllergenOptions.Where(a => a.IsSelected).Select(a => a.Value));
                    }
                };
                AllergenOptions.Add(item);
            }
        }

        #endregion

        #region Get Products Selection
        private void GetProductSelections()
        {
            ProductSelections.Clear();

            // Map former selections: Product_ID -> ProductInDish
            var existing = (SelectedDish?.ProductUsage ?? new List<ProductInDish>())
                           .ToDictionary(x => x.Product_ID);

            // Show ALL inventory; preselect + prefill amounts for ones already in the dish
            foreach (var p in AvailableProducts)
            {
                var row = new SelectableProduct(p.Product_ID, p.Product_Name, p.Quantity_Available);

                if (existing.TryGetValue(p.Product_ID, out var link))
                {
                    row.IsSelected = true;
                    row.AmountUsage = link.Amount_Usage;
                }

                ProductSelections.Add(row);
            }
        }
        #endregion
    }
}
