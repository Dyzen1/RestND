using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using RestND.utilities;
using RestND.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;

public partial class DiscountViewModel : ObservableObject
{
    #region Services
    private readonly DiscountService _discountService;
    private readonly DiscountValidator _discountValidator = new();
    #endregion

    #region Fields

    [ObservableProperty]
    public ObservableCollection<Discount> discounts = new();

    [ObservableProperty]
    public Discount selectedDiscount;

    [ObservableProperty]
    public Discount newDiscount = new();

    [ObservableProperty]
    public string newDiscountName = string.Empty;

    [ObservableProperty]
    public string newDiscountPercentage = string.Empty;

    [ObservableProperty] private string formErrorMessage;

    #endregion

    #region Constructor
    public DiscountViewModel()
    {
        _discountService = new DiscountService();
        LoadDiscounts();
    }

    #endregion

    #region On Change
    partial void OnSelectedDiscountChanged(Discount value)
    {
        DeleteDiscountCommand.NotifyCanExecuteChanged();
        UpdateDiscountCommand.NotifyCanExecuteChanged();
        AddDiscountCommand.NotifyCanExecuteChanged();
    }

    #endregion

    #region Load

    [RelayCommand]
    public void LoadDiscounts()
    {
        Discounts.Clear();
        var dbDiscounts = _discountService.GetAll();
        foreach (var discount in dbDiscounts)
            Discounts.Add(discount);
    }

    #endregion

    #region Add

    [RelayCommand]
    public void AddDiscount()
    {
        // 1) Validate full form (Employee-style)
        if (!_discountValidator.ValidateForAdd(
                NewDiscountName,
                NewDiscountPercentage,
                Discounts,
                out var parsedPercentage,
                out var err))
        {
            FormErrorMessage = err;
            return;
        }
        // Passed validation — assemble the new Dish
        var discount = new Discount 
        {   
            Discount_Name = NewDiscountName.Trim(), 
            Discount_Percentage = double.Parse(NewDiscountPercentage),
            Is_Active = true
        };

        bool success = _discountService.Add(discount);
        if (!success) return;

        LoadDiscounts();
        NewDiscountName = string.Empty;
        NewDiscountPercentage = string.Empty;
        FormErrorMessage = string.Empty;
    }

    #endregion

    #region Update

    [RelayCommand]
    public void UpdateDiscount()
    {
        // Validate entire form (Employee-style)
        var existingDiscs = _discountService.GetAll(); // for "exists by id" + name uniqueness (exclude current)
        if (!_discountValidator.ValidateForUpdate(
                SelectedDiscount,
                existingDiscs,
                out var err))
        {
            FormErrorMessage = err;
            return;
        }
        // passed validation:
        FormErrorMessage = string.Empty;
        SelectedDiscount.Is_Active = true;

        bool success = _discountService.Update(SelectedDiscount);
        if (success)
        {
            LoadDiscounts();
        }
    }

    #endregion

    #region Delete

    [RelayCommand]
    public void DeleteDiscount()
    {
        bool success = _discountService.Delete(SelectedDiscount);
        if (success)
        {
            LoadDiscounts();
        }
    }

    #endregion

    #region CanExecute
    //private bool CanModifyDiscount()
    //{
    //    if (SelectedDiscount == null) return false;
    //    DiscountValidationErrors = DiscountValidator.ValidateFields(
    //        SelectedDiscount,
    //        Discounts.Where(d => d.Discount_ID != SelectedDiscount.Discount_ID).ToList());
    //    return !DiscountValidationErrors.Any();
    //}

    //private bool CanAddDiscount()
    //{
    //    if (NewDiscount == null) return false;
    //    DiscountValidationErrors = DiscountValidator.ValidateFields(NewDiscount, Discounts.ToList());
    //    return !DiscountValidationErrors.Any();
    //}

    #endregion
}
