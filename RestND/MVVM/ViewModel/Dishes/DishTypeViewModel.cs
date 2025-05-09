using CommunityToolkit.Mvvm.ComponentModel;
using RestND.Data;
using RestND.MVVM.Model;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public partial class DishTypeViewModel : ObservableObject
{

    #region Services
    // Service for handling DishType database operations
    private readonly DishTypeServices _dishTypeService;
    #endregion

    #region propery
    public List<DishType> DishTypeList { get; set; }

    #endregion

    #region Constractor
    public DishTypeViewModel()
    {
        _dishTypeService = new DishTypeServices();
        DishTypeList = _dishTypeService.GetAll();
    }
    #endregion

    #region Observable Properties
    // List of dish types displayed in the UI
    [ObservableProperty]
    private ObservableCollection<DishType> dishTypes = new();
    // The dish type currently selected in the UI
    [ObservableProperty]
    private DishType selectedDishType;
    // Called automatically when SelectedDishType changes
    partial void OnSelectedDishTypeChanged(DishType value)
    {
        DeleteDishTypeCommand.NotifyCanExecuteChanged();
        UpdateDishTypeCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region Load Method
   public DishTypeViewModel(DishTypeServices dishTypeService)
    {
        _dishTypeService = dishTypeService;
        LoadDishTypes();
    }
    // Loads all dish types from the database
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
    // Command for deleting the selected dish type
    [RelayCommand(CanExecute = nameof(CanModifyDishType))]
    private void DeleteDishType()
    {
        if (SelectedDishType != null)
        {
            bool success = _dishTypeService.Delete(SelectedDishType.DishType_ID);

            if (success)
            {
                DishTypes.Remove(SelectedDishType);
            }
        }
    }
    #endregion

    #region Update
    // Command for updating the selected dish type
    [RelayCommand(CanExecute = nameof(CanModifyDishType))]
    private void UpdateDishType()
    {
        if (SelectedDishType != null)
        {
            bool success = _dishTypeService.Update(SelectedDishType);

            if (success)
            {
                LoadDishTypes();
            }
        }
    }
    #endregion
    
    #region Add
    // Command for adding a new dish type
    [RelayCommand]
    private void AddDishType()
    {
        if (SelectedDishType != null)
        {
            bool success = _dishTypeService.Add(SelectedDishType);

            if (success)
            {
                LoadDishTypes();
                SelectedDishType = new DishType();
            }
        }
    }
    #endregion

    #region Helper Methods
    // Helper method: checks if a dish type is selected (used for button enabling)
    private bool CanModifyDishType()
    {
        return SelectedDishType != null;
    }
    #endregion



 
}