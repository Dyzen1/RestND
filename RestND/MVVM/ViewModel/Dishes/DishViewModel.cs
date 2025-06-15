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
        [ObservableProperty] private ObservableCollection<ProductInDish> selectedProducts = new();

        //FOR THE CHECK BOX OPTIONS:
        [ObservableProperty] private ObservableCollection<string> selectedAllergenNotes = new();
        [ObservableProperty] private ObservableCollection<SelectableItem<string>> allergenOptions = new();

        [ObservableProperty] private Dictionary<string, List<string>> dishValidationErrors = new();

        [ObservableProperty] private Dish newDish = new();
        [ObservableProperty] private Dish selectedDish;
        [ObservableProperty] private Inventory selectedAvailableProduct;
        [ObservableProperty] private int productAmountUsage;
        #endregion

        #region Constructor
        public DishViewModel()
        {
            _dishTypeService = new DishTypeServices();
            _dishService = new DishServices();
            _productService = new ProductService();
            _hub = App.DishHub;
            
            DishTypes = new ObservableCollection<DishType>(_dishTypeService.GetAll());

            LoadDishes();
            LoadAvailableProducts();
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
                                match.Availability_Status = dish.Availability_Status;
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

        [RelayCommand]
        private void LoadAvailableProducts()
        {
            AvailableProducts.Clear();
            var dbProducts = _productService.GetAll();
            foreach (var product in dbProducts)
                AvailableProducts.Add(product);
        }

        private List<ProductInDish> CloneSelectedProducts() =>
            SelectedProducts.Select(p => new ProductInDish
            {
                Product_ID = p.Product_ID,
                Amount_Usage = p.Amount_Usage
            }).ToList();


        [RelayCommand(CanExecute = nameof(CanAddDish))]
        private async Task AddDish()
        {
            NewDish.ProductUsage = CloneSelectedProducts();
            NewDish.Allergen_Notes = string.Join(",", SelectedAllergenNotes);

            if (string.IsNullOrWhiteSpace(NewDish.Dish_Name) ||
                NewDish.Dish_Price <= 0 ||
                NewDish.Dish_Type == null ||
                NewDish.ProductUsage == null || !NewDish.ProductUsage.Any())
            {
                MessageBox.Show("Please fill in all required fields and add at least one product.");
                return;
            }

            bool success = _dishService.Add(NewDish);

            if (success)
            {
                await _hub.SendAsync("NotifyDishUpdate", NewDish, "add");
                NewDish = new Dish();
                SelectedProducts.Clear();
                SelectedAllergenNotes.Clear();
            }
            else
            {
                MessageBox.Show("Failed to save dish. Please try again.");
            }
        }


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

    }
}
