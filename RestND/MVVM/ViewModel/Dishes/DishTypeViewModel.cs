using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND;
using RestND.Data;
using RestND.MVVM.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

public partial class DishTypeViewModel : ObservableObject
{
    #region Services
    private readonly DishTypeServices _dishTypeService;
    private readonly HubConnection _hub;
    #endregion

    #region Properties
    public List<DishType> DishTypeList { get; set; }
    #endregion

    #region Observable Properties
    [ObservableProperty] private ObservableCollection<DishType> dishTypes = new();
    [ObservableProperty] private DishType selectedDishType;
    #endregion

    #region Constructors
    public DishTypeViewModel()
    {
        _dishTypeService = new DishTypeServices();
        _hub = App.DishTypeHub;

        InitializeHubSubscriptions();
        LoadDishTypes();

        // If you still need the raw list:
        DishTypeList = _dishTypeService.GetAll();
    }

    // DI-friendly overload
    public DishTypeViewModel(DishTypeServices dishTypeService)
    {
        _dishTypeService = dishTypeService;
        _hub = App.DishHub;

        InitializeHubSubscriptions();
        LoadDishTypes();

        DishTypeList = _dishTypeService.GetAll();
    }
    #endregion

    #region Hub
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
        }
    }
    #endregion

    #region Update
    [RelayCommand(CanExecute = nameof(CanModifyDishType))]
    private async Task UpdateDishType()
    {
        if (SelectedDishType == null) return;

        bool success = _dishTypeService.Update(SelectedDishType);

        if (success)
        {
            await _hub.SendAsync("NotifyDishTypeUpdate", SelectedDishType, "update");
            await App.DishHub.SendAsync("NotifyDishUpdate", null, "dishType-changed");
            LoadDishTypes(); 
        }
    }
    #endregion

    #region Add
    [RelayCommand]
    private async Task AddDishType()
    {
        // You were using SelectedDishType for Add; keeping that behavior.
        if (SelectedDishType == null) return;

        bool success = _dishTypeService.Add(SelectedDishType);

        if (success)
        {
            await _hub.SendAsync("NotifyDishTypeUpdate", SelectedDishType, "add");
            await App.DishHub.SendAsync("NotifyDishUpdate", null, "dishType-changed");
            SelectedDishType = new DishType();
             LoadDishTypes();
        }
    }
    #endregion

    #region Helper Methods
    private bool CanModifyDishType() => SelectedDishType != null;
    #endregion
}
