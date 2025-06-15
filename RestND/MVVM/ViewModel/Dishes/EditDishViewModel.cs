using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using RestND.MVVM.ViewModel.Dishes;
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
        private readonly DishServices _dishService = new();
        private readonly DishTypeServices _dishTypeService = new();
        private readonly HubConnection _hub;
        #endregion

        #region Observable properties
        [ObservableProperty] private Dish selectedDish;
        [ObservableProperty] private ObservableCollection<DishType> dishTypes = new(); 
        [ObservableProperty] private ObservableCollection<SelectableItem<string>> allergenOptions = new();
        private readonly List<string> _allPossibleAllergens = new()
        {
            "Contains Gluten/Wheat.",
            "Contains Peanuts.",
            "Contains Tree Nuts.",
            "Contains Milk Or Dairy Ingredients.",
            "Contains Eggs Or Egg-Derived Ingredients.",
            "Contains Soy Or Soy-Derived Ingredients.",
            "Contains Fish.",
            "Contains Shellfish.",
            "Contains Sesame.",
            "Contains Mustard.",
            "Contains Celery.",
            "Contains Sulfites."
        };
        #endregion

        #region Constructor
        public EditDishViewModel(Dish dishToEdit)
        {
            _hub = App.DishHub;

            SelectedDish = new Dish
            {
                Dish_ID = dishToEdit.Dish_ID,
                Dish_Name = dishToEdit.Dish_Name,
                Dish_Price = dishToEdit.Dish_Price,
                Availability_Status = dishToEdit.Availability_Status,
                Dish_Type = dishToEdit.Dish_Type,
                Allergen_Notes = dishToEdit.Allergen_Notes
            };

            // saving the formr user's selection of dish type.
            DishTypes = new ObservableCollection<DishType>(_dishTypeService.GetAll());
            if (SelectedDish.Dish_Type != null)
            {
                var matchingType = DishTypes.FirstOrDefault(dt => dt.DishType_Name == SelectedDish.Dish_Type.DishType_Name);
                if (matchingType != null)
                    SelectedDish.Dish_Type = matchingType;
            }

            AllergenOptions = new ObservableCollection<SelectableItem<string>>();

            // adding the selected allergens to the options list.
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
                        SelectedDish.Allergen_Notes = string.Join(",",
                            AllergenOptions.Where(a => a.IsSelected).Select(a => a.Value));
                    }
                };
                AllergenOptions.Add(item); 
            }
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
            //SelectedDish.Allergen_Notes = SelectedAllergenNotes; 

            bool success = _dishService.Update(SelectedDish);
            if (success)
            {
                await _hub.SendAsync("NotifyDishUpdate", SelectedDish, "update");
                MessageBox.Show("Dish updated successfully.", "Success");
            }
            else
            {
                MessageBox.Show("Failed to update dish.", "Error");
            }
        }
        #endregion
    }
}
