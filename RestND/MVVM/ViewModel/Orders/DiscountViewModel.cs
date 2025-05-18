using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.MVVM.Model.Orders;
using RestND.Validations;
using RestND.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

public partial class DiscountViewModel : ObservableObject
{
    #region Services
    private readonly DiscountService _discountService;
    #endregion

    #region Fields

    [ObservableProperty]
    public ObservableCollection<Discount> discounts = new();

    [ObservableProperty]
    public Discount selectedDiscount;

    [ObservableProperty]
    public Discount newDiscount = new();

    [ObservableProperty]
    public Dictionary<string, List<string>> discountValidationErrors = new();

    partial void OnSelectedDiscountChanged(Discount value)
    {
        DeleteDiscountCommand.NotifyCanExecuteChanged();
        UpdateDiscountCommand.NotifyCanExecuteChanged();
    }

    partial void OnNewDiscountChanged(Discount value)
    {
        AddDiscountCommand.NotifyCanExecuteChanged();
    }

    #endregion

    #region Constructor
    public DiscountViewModel()
    {
        _discountService = new DiscountService();
        LoadDiscounts();
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
    [RelayCommand(CanExecute = nameof(CanAddDiscount))]
    public void AddDiscount()
    {
        bool success = _discountService.Add(NewDiscount);
        if (!success) return;

        LoadDiscounts();
        NewDiscount = new Discount();
        discountValidationErrors.Clear();
    }

    #endregion

    #region Update
    [RelayCommand(CanExecute = nameof(CanModifyDiscount))]
    public void UpdateDiscount()
    {
        bool success = _discountService.Update(SelectedDiscount);
        if (success)
        {
            LoadDiscounts();
            discountValidationErrors.Clear();
        }
    }
    #endregion

    #region Delete
    [RelayCommand(CanExecute = nameof(CanModifyDiscount))]
    public void DeleteDiscount()
    {
        bool success = _discountService.Delete(SelectedDiscount.Discount_ID);
        if (success)
        {
            LoadDiscounts();
        }
    }
    #endregion

    #region CanExecute
    private bool CanModifyDiscount()
    {
        if (SelectedDiscount == null) return false;
        discountValidationErrors = DiscountValidator.ValidateFields(
            SelectedDiscount,
            Discounts.Where(d => d.Discount_ID != SelectedDiscount.Discount_ID).ToList());
        return !discountValidationErrors.Any();
    }

    private bool CanAddDiscount()
    {
        if (NewDiscount == null) return false;
        discountValidationErrors = DiscountValidator.ValidateFields(NewDiscount, Discounts.ToList());
        return !discountValidationErrors.Any();
    }
    #endregion
}
