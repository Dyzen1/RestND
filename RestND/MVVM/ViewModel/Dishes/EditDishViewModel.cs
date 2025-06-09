using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestND.MVVM.ViewModel
{
    public partial class EditDishViewModel : ObservableObject
    {
        private readonly DishServices _dishService = new();
        private readonly DishTypeServices _dishTypeService = new();
        private readonly HubConnection _hub;

        [ObservableProperty] private Dish selectedDish;
        [ObservableProperty] private ObservableCollection<DishType> dishTypes;
        [ObservableProperty] private ObservableCollection<AllergenNotes> allergenNotes;
        [ObservableProperty] private string selectedAllergenNotes;

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

            DishTypes = new ObservableCollection<DishType>(_dishTypeService.GetAll());
            AllergenNotes = new ObservableCollection<AllergenNotes>();
            SelectedAllergenNotes =  SelectedDish.Allergen_Notes;
        }

        [RelayCommand]
        private async Task UpdateDish()
        {
            //SelectedDish.Allergen_Notes = SelectedAllergenNotes.ToList();

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
    }
}
