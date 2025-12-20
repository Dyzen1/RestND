using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model.Dishes;
using RestND.Validations;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RestND.MVVM.ViewModel.Dishes
{
    public partial class SoftDrinkViewModel : ObservableObject
    {
        #region Services
        private readonly SoftDrinkServices _service = new();
        private readonly SoftDrinkValidator _validator = new();
        #endregion

        #region Observable Properties
        [ObservableProperty] private int drinkId;
        [ObservableProperty] private string? drinkName;
        [ObservableProperty] private double drinkPrice;
        [ObservableProperty] private int quantity;
        [ObservableProperty] private bool isActive = true;

        [ObservableProperty] private string? errorMessage;

        [ObservableProperty] private ObservableCollection<SoftDrink> softDrinks = new();
        [ObservableProperty] private SoftDrink? selectedSoftDrink;
        #endregion

        #region Ctor
        public SoftDrinkViewModel()
        {
            LoadSoftDrinks();
        }
        #endregion

        #region On Change
        // When a row is selected in the grid, we load it into the form
        partial void OnSelectedSoftDrinkChanged(SoftDrink? value)
        {
            if (value is null) return;
            LoadFrom(value);
        }
        #endregion

        #region Load Method
        private void LoadFrom(SoftDrink d)
        {
            DrinkId = d.Drink_ID;
            DrinkName = d.Drink_Name;
            DrinkPrice = d.Drink_Price;
            Quantity = d.Quantity;
            IsActive = d.Is_Active;
            ErrorMessage = null;
        }
        #endregion

        #region Clear form method
        private void ClearForm()
        {
            SelectedSoftDrink = null;
            DrinkId = 0;
            DrinkName = string.Empty;
            DrinkPrice = 0;
            Quantity = 0;
            IsActive = true;
            ErrorMessage = null;
        }
        #endregion

        #region Relay commands
        [RelayCommand]
        private void LoadSoftDrinks()
        {
            SoftDrinks = new ObservableCollection<SoftDrink>(_service.GetAll());
        }

        // Create new soft drink
        [RelayCommand]
        private void Save()
        {
            ErrorMessage = null;

            if (DrinkId != 0)
            {
                ErrorMessage = "Item already exists. Use Update instead.";
                return;
            }

            var model = new SoftDrink(
                drink_ID: 0,
                drink_Name: DrinkName,
                drink_Price: DrinkPrice,
                quantity: Quantity,
                is_Active: true
            );

            if (!_validator.ValidateForAdd(model, out string err))
            {
                ErrorMessage = err;
                return;
            }

            if (!_service.Add(model))
            {
                ErrorMessage = "Create failed.";
                return;
            }

            // Refresh and select the newly created row
            LoadSoftDrinks();
            var created = SoftDrinks.OrderByDescending(d => d.Drink_ID).FirstOrDefault();
            if (created is not null)
            {
                SelectedSoftDrink = created;
                LoadFrom(created);
            }
            else
            {
                ClearForm();
            }
        }

        // Update an existing soft drink
        [RelayCommand]
        private void Update()
        {
            ErrorMessage = null;

            if (DrinkId == 0)
            {
                ErrorMessage = "Select a drink to update.";
                return;
            }

            var model = new SoftDrink(DrinkId, DrinkName, DrinkPrice, Quantity, IsActive);

            if (!_validator.ValidateForUpdate(model, out string err))
            {
                ErrorMessage = err;
                return;
            }

            if (!_service.Update(model))
            {
                ErrorMessage = "Update failed.";
                return;
            }

            LoadSoftDrinks();
            var updated = SoftDrinks.FirstOrDefault(d => d.Drink_ID == DrinkId);
            if (updated is not null)
            {
                SelectedSoftDrink = updated;
                LoadFrom(updated);
            }
        }

        // Soft delete (your service flips Is_Active = false)
        [RelayCommand]
        private void Delete()
        {
            if (DrinkId == 0) return;

            var confirm = MessageBox.Show(
                $"Delete '{DrinkName}'?",
                "Confirm delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            ErrorMessage = null;

            var model = new SoftDrink(DrinkId, DrinkName, DrinkPrice, Quantity, IsActive);
            if (!_service.Delete(model))
            {
                ErrorMessage = "Delete failed.";
                return;
            }

            LoadSoftDrinks();
            ClearForm();
        }
        #endregion
    }
}
