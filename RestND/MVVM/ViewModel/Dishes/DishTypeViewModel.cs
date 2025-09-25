using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using RestND.Validations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

public partial class DishTypeViewModel : ObservableObject
{
    #region Services
    private readonly DishTypeServices _dishTypeService;
    private readonly DishTypeValidator _validator = new();
    private readonly HubConnection _hub;
    #endregion

    #region Observable Properties
    [ObservableProperty] private ObservableCollection<DishType> dishTypes = new();
    [ObservableProperty] public DishType newType = new();
    [ObservableProperty] private DishType selectedDishType;
    [ObservableProperty] private string errorMessage;
    [ObservableProperty] private string newTypeInput;
    #endregion

    #region Constructors
    public DishTypeViewModel()
    {
        _dishTypeService = new DishTypeServices();
        _hub = App.DishTypeHub;

        LoadDishTypes();
    }
    #endregion

    #region Initialize SignalR
    private void InitializeHubSubscriptions()
    {
        // Listen for dish type updates from other clients
        _hub.On<DishType, string>("ReceiveDishTypeUpdate", (dishType, action) =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var match = DishTypes.FirstOrDefault(dt => dt.DishType_ID == dishType.DishType_ID);

                switch (action)
                {
                    case "add":
                        if (match == null)
                            DishTypes.Add(dishType);
                        break;

                    case "update":
                        if (match != null)
                        {
                            match.DishType_Name = dishType.DishType_Name;
                            match.Is_Active = dishType.Is_Active;
                        }
                        break;

                    case "delete":
                        if (match != null)
                            DishTypes.Remove(match);
                        break;
                }
            });
        });
    }
    #endregion

    #region On change
    partial void OnSelectedDishTypeChanged(DishType value)
    {
        DeleteDishTypeCommand.NotifyCanExecuteChanged();
        UpdateDishTypeCommand.NotifyCanExecuteChanged();
        AddDishTypeCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region Load Method
    [RelayCommand]
    private void LoadDishTypes()
    {
        DishTypes.Clear();
        var dbDishTypes = _dishTypeService.GetAll();
        foreach (var dishType in dbDishTypes)
            DishTypes.Add(dishType);
    }
    #endregion

    #region Delete
    [RelayCommand(CanExecute = nameof(CanModifyDishType))]
    private async Task DeleteDishType()
    {
        if (SelectedDishType == null) return;

        bool success = _dishTypeService.Delete(SelectedDishType);

        if (success)
        {
            await _hub.SendAsync("NotifyDishTypeUpdate", SelectedDishType, "delete");
            await App.DishHub.SendAsync("NotifyDishUpdate", null, "dishType-changed");
            LoadDishTypes();
            InitializeHubSubscriptions();
        }
    }
    #endregion

    #region Update
    [RelayCommand(CanExecute = nameof(CanModifyDishType))]
    private async Task UpdateDishType()
    {
        // Validate entire form (Employee-style)
        var existingTypes = _dishTypeService.GetAll(); // for "exists by id" + name uniqueness (exclude current)
        if (!_validator.ValidateForUpdate(
                SelectedDishType,
                existingTypes,
                out var err))
        {
            ErrorMessage = err;
            return;
        }
        // passed validation:
        ErrorMessage = string.Empty;
        SelectedDishType.Is_Active = true;

        bool success = _dishTypeService.Update(SelectedDishType);

        if (success)
        {
            await _hub.SendAsync("NotifyDishTypeUpdate", SelectedDishType, "update");
            await App.DishHub.SendAsync("NotifyDishUpdate", null, "dishType-changed");
            LoadDishTypes();
            InitializeHubSubscriptions();
        }
    }
    #endregion

    #region Add
    [RelayCommand]
    private async Task AddDishType()
    {
        // 1) Validate full form (Employee-style)
        if (!_validator.ValidateForAdd(
                NewTypeInput,
                DishTypes,
                out var err))
        {
            ErrorMessage = err;
            return;
        }

        var type = new DishType
        {
            DishType_Name = NewTypeInput.Trim(),
            Is_Active = true
        };

        bool success = _dishTypeService.Add(type);

        if (success)
        {
            await _hub.SendAsync("NotifyDishTypeUpdate", SelectedDishType, "add");
            await App.DishHub.SendAsync("NotifyDishUpdate", null, "dishType-changed");
            LoadDishTypes();
            InitializeHubSubscriptions();
            ErrorMessage = string.Empty;
            NewTypeInput = string.Empty;
        }
    }
    #endregion

    #region Helper Methods
    private bool CanModifyDishType() => SelectedDishType != null;
    #endregion
}
