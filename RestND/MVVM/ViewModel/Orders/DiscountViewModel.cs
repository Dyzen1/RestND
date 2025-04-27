using RestND.MVVM.Model;

public partial class DiscountViewModel : ObservableObject
{
    #region Services

    private readonly DiscountService _discountService;
    #endregion
    
    #region Fields

    [ObservableProperty]
    public ObservableCollection<Discount> discounts = new ObservableCollection<Discount>();

     [ObservableProperty]
    public Discount selectedDiscount;

    [ObservableProperty]
    public Discount newDiscount = new Discount();

         partial void OnSelectedDiscountChanged(Discount value)
        {
            DeleteDiscountCommand.NotifyCanExecuteChanged();
            UpdateDiscountCommand.NotifyCanExecuteChanged();
        } 


    #endregion

    #region Constructors
    public DiscountViewModel()
    {
        _discountService = new DiscountService();
        LoadDiscounts();
    }
    #endregion

    #region Load Discounts

        [RelayCommand]
    public void LoadDiscounts(){

        discounts.Clear();
        var dbDiscounts = _discountService.GetAll();
        foreach (var discount in dbDiscounts)
            discounts.Add(discount);    

    }
    #endregion

    #region Add Discount

    [RelayCommand]
    public void AddDiscount(){
        bool success = _discountService.Add(newDiscount);
        if(success){
            LoadDiscounts();
            newDiscount = new Discount();
        }
    }
    #endregion

    #region Update Discount

    [RelayCommand (CanExecute = nameof(CanModifyProduct))]
    public void UpdateDiscount(){
        bool success = _discountService.Update(selectedDiscount);
        if(success){
            LoadDiscounts();
        }
    }
    #endregion  

    #region Delete Discount 

    [RelayCommand (CanExecute = nameof(CanModifyProduct))] 
    public void DeleteDiscount(){
        bool success = _discountService.Delete(selectedDiscount.Discount_ID);
        if(success){
            LoadDiscounts();
        }
    }
    #endregion


}
