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
        private readonly DishServices _dishService;
        private readonly DishTypeServices _dishTypeService;
        private readonly ProductService _productService;
        private readonly HubConnection _hub;
        private readonly AllergenNotes _allergenNotes = new AllergenNotes();
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
            //LoadProducts();
            LoadNotes();

            _hub.On<Dish, string>("ReceiveDishUpdate", (dish, action) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var match = Dishes.FirstOrDefault(d => d.Dish_ID == dish.Dish_ID);

                    switch (action)
                    {
                        case "add":
                            if (match == null)
                                Dishes.Add(dish);
                            break;
                        case "update":
                            if (match != null)
                            {
                                match.Dish_Name = dish.Dish_Name;
                                match.Dish_Price = dish.Dish_Price;
                                match.Allergen_Notes = dish.Allergen_Notes;
                                match.Dish_Type = dish.Dish_Type;
                            }
                            break;
                        case "delete":
                            if (match != null)
                                Dishes.Remove(match);
                            break;
                    }
                });
            });
            LoadDishes();
        }
        #endregion

        #region On change
        partial void OnSelectedDishChanged(Dish value)
        {
            DeleteDishCommand.NotifyCanExecuteChanged();
            AddDishCommand.NotifyCanExecuteChanged();
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

        //[RelayCommand]
        //private void LoadProducts()
        //{
        //    ProductOptions.Clear();

        //    foreach (var product in SelectedProductsInDish)
        //    {
        //        var productInDish = new ProductInDish
        //        {
        //            Product_ID = product.Product_ID,
        //            Dish_ID = product.Dish_ID,
        //            Product_Name = product.Product_Name,
        //            Amount_Usage = 0
        //        };
        //        var item = new SelectableItem<ProductInDish>(productInDish)
        //        {
        //            IsSelected = false 
        //        };

        //        ProductOptions.Add(item);
        //    }
        //}

        //private List<ProductInDish> CloneSelectedProducts() =>
        //    SelectedProductsInDish.Select(p => new ProductInDish
        //    {
        //        Product_ID = p.Product_ID,
        //        Amount_Usage = p.Amount_Usage
        //    }).ToList();


        [RelayCommand(CanExecute = nameof(CanAddDish))]
        private async Task AddDish()
        {
            // Allergens from checkboxes
            NewDish.Allergen_Notes = string.Join(",", SelectedAllergenNotes);

            // Build ProductUsage from the grid rows the user checked + filled
            var chosen = ProductSelections
                .Where(x => x.isSelected && x.amountUsage > 0)
                .Select(x => new ProductInDish
                {
                    Product_ID = x.Product_ID,
                    Product_Name = x.Product_Name,
                    Amount_Usage = x.AmountUsage,
                    Dish_ID = NewDish.Dish_ID // keep 0 if DB assigns
                })
                .ToList();

            NewDish.ProductUsage = chosen;

            // Basic validation
            if (string.IsNullOrWhiteSpace(NewDish.Dish_Name) ||
                NewDish.Dish_Price <= 0 ||
                NewDish.Dish_Type == null ||
                NewDish.ProductUsage == null || !NewDish.ProductUsage.Any())
            {
                MessageBox.Show("Please fill all fields and add at least one product with an amount > 0.");
                return;
            }

            bool success = _dishService.Add(NewDish);

            if (success)
            {
                await _hub.SendAsync("NotifyDishUpdate", NewDish, "add");

                // reset the form
                NewDish = new Dish();
                SelectedAllergenNotes.Clear();
                foreach (var sp in productSelections)
                {
                    sp.isSelected = false;
                    sp.amountUsage = 0;
                }
            }
            else
            {
                MessageBox.Show("Failed to save dish. Please try again.");
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
            productSelections = new ObservableCollection<SelectableProduct>(
                AvailableProducts.Select(p =>
                    new SelectableProduct(p.Product_ID, p.Product_Name, p.Quantity_Available))
            );
        }
        #endregion
    }
}
