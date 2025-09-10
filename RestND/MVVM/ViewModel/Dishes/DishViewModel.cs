using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using RestND.MVVM.ViewModel.Dishes;
using RestND.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestND.MVVM.ViewModel
{
    public partial class DishViewModel : ObservableObject
    {
        #region Services
        private readonly DishValidator _dishValidator = new();
        private readonly DishServices _dishService;
        private readonly DishTypeServices _dishTypeService;
        private readonly ProductService _productService;
        private readonly ProductInDishService _productInDishService = new();
        private readonly AllergenNotes _allergenNotes = new AllergenNotes();
        private readonly HubConnection _hub;
        #endregion

        #region Observable properties
        [ObservableProperty] private ObservableCollection<Dish> dishes = new();
        [ObservableProperty] private ObservableCollection<DishType> dishTypes = new();
        [ObservableProperty] private ObservableCollection<Inventory> availableProducts = new();
        [ObservableProperty] private ObservableCollection<SelectableProduct> productSelections = new();

        //FOR THE CHECK BOX OPTIONS:
        [ObservableProperty] private ObservableCollection<string> selectedAllergenNotes = new();
        [ObservableProperty] private ObservableCollection<SelectableItem<string>> allergenOptions = new();

        [ObservableProperty] private Dish newDish = new();
        [ObservableProperty] private Dish selectedDish;

        [ObservableProperty] private string dishErrorMessage;
        [ObservableProperty] private string newDishNameInput;
        [ObservableProperty] private string newDishPriceInput;
        [ObservableProperty] private DishType newDishTypeInput;

        #endregion

        #region Constructor
        public DishViewModel()
        {
            _dishTypeService = new DishTypeServices();
            _dishService = new DishServices();
            _productService = new ProductService();
            _hub = App.DishHub;

            DishTypes = new ObservableCollection<DishType>(_dishTypeService.GetAll());
            AvailableProducts = new ObservableCollection<Inventory>(_productService.GetAll());

            LoadDishes();
            RefreshProductSelections();
            LoadNotes();

            _hub.On<Dish, string>("ReceiveDishUpdate", (dish, action) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {

                    if (action == "product-deleted")
                    {
                        LoadDishes(); 
                        return;
                    }

                    var match = Dishes.FirstOrDefault(d => d.Dish_ID == dish.Dish_ID);

                    switch (action)
                    {
                        case "add":
                            if (match == null)
                            {
                                Dishes.Add(dish);
                                LoadDishes();
                            }
                                
                            break;
                        case "delete":
                            if (match != null)
                                Dishes.Remove(match);
                            break;
                    }
                });
            });
            //LoadDishes();
        }
        #endregion

        #region On change
        partial void OnSelectedDishChanged(Dish value)
        {
            DeleteDishCommand.NotifyCanExecuteChanged();
            AddDishCommand.NotifyCanExecuteChanged();

            if (value == null) return;

            // Load link rows only if missing/empty
            if (value.ProductUsage == null || value.ProductUsage.Count == 0)
            {
                var rows = _productInDishService.GetProductsInDish(value.Dish_ID) ?? new List<ProductInDish>();

                // 🔧 Patch missing names using inventory (handles null/empty Product_Name)
                var nameById = AvailableProducts
                    .ToDictionary(p => p.Product_ID, p => p.Product_Name);

                foreach (var r in rows)
                {
                    if (string.IsNullOrWhiteSpace(r.Product_Name) &&
                        nameById.TryGetValue(r.Product_ID, out var name))
                    {
                        r.Product_Name = name;
                    }
                }

                value.ProductUsage = rows;
            }
        }


        #endregion

        #region Relay commands

        [RelayCommand]
        private void LoadDishes()
        {
            Dishes.Clear();
            var dbDishes = _dishService.GetAll();
            foreach (var dish in dbDishes)
                Dishes.Add(dish);
        }

        [RelayCommand]
        private void LoadNotes()
        {
            AllergenOptions.Clear();

            foreach (var note in _allergenNotes.Allergens)
            {
                var item = new SelectableItem<string>(note)
                {
                    IsSelected = SelectedAllergenNotes.Contains(note)
                };

                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(SelectableItem<string>.IsSelected))
                    {
                        if (item.IsSelected && !SelectedAllergenNotes.Contains(item.Value))
                            SelectedAllergenNotes.Add(item.Value);
                        else if (!item.IsSelected && SelectedAllergenNotes.Contains(item.Value))
                            SelectedAllergenNotes.Remove(item.Value);
                    }
                };

                AllergenOptions.Add(item); 
            }
        }


        [RelayCommand(CanExecute = nameof(CanAddDish))]
        private async Task AddDish()
        {
            // 1) Validate full form (Employee-style)
            if (!_dishValidator.ValidateForAdd(
                    NewDishNameInput,
                    NewDishPriceInput,
                    NewDishTypeInput,
                    AllergenOptions,
                    ProductSelections,
                    Dishes,                    // existing dishes for uniqueness check
                    out var parsedPrice,
                    out var err))
            {
                DishErrorMessage = err;
                return;
            }

            // 2) Build chosen products from grid
            var chosen = ProductSelections
                .Where(x => x.IsSelected && x.AmountUsage > 0)
                .Select(x => new ProductInDish
                {
                    Product_ID = x.Product_ID,
                    Product_Name = x.Product_Name,
                    Amount_Usage = x.AmountUsage,
                    Dish_ID = 0 // DB assigns on insert
                })
                .ToList();

            // 3) Passed validation — assemble the new Dish
            DishErrorMessage = string.Empty;

            NewDish.ProductUsage = chosen;
            NewDish.Dish_Name = (NewDishNameInput ?? string.Empty).Trim();
            NewDish.Dish_Price = parsedPrice;
            NewDish.Dish_Type = NewDishTypeInput;
            NewDish.Is_Active = true;
            NewDish.Allergen_Notes = string.Join(", ",
                AllergenOptions.Where(a => a.IsSelected).Select(a => a.Value));

            // 4) Persist
            bool success = _dishService.Add(NewDish);
            if (success)
            {
                await _hub?.SendAsync("NotifyDishUpdate", NewDish, "add");

                // 5) Reset form
                NewDish = new Dish();

                NewDishNameInput = string.Empty;
                NewDishPriceInput = string.Empty;
                NewDishTypeInput = null;

                foreach (var a in AllergenOptions) a.IsSelected = false;
                SelectedAllergenNotes?.Clear(); // keep if you still maintain this collection

                foreach (var sp in ProductSelections)
                {
                    sp.IsSelected = false;
                    sp.AmountUsage = 0;
                }


            }
       
        }



        [RelayCommand(CanExecute = nameof(CanModifyDish))]
        private async Task DeleteDish()
        {
            if (SelectedDish != null)
            {
                bool success = _dishService.Delete(SelectedDish);
                if (success)
                {
                    await _hub.SendAsync("NotifyDishUpdate", SelectedDish, "delete");
                    Dishes.Remove(SelectedDish);
                    LoadDishes();
                }
            }
        }
        #endregion

        #region Can modify
        private bool CanModifyDish() => SelectedDish != null;
        private bool CanAddDish() => true;
        #endregion

        #region Refresh Products Selection
        private void RefreshProductSelections()
        {
            ProductSelections = new ObservableCollection<SelectableProduct>(
                AvailableProducts.Select(p =>
                    new SelectableProduct(p.Product_ID, p.Product_Name, p.Quantity_Available))
            );
        }
        #endregion
    }
}
