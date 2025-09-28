using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestND.MVVM.ViewModel.Orders
{
    public partial class OrderViewModel : ObservableObject
    {
        #region Services
        private readonly DishServices _dishSvc = new();
        private readonly DishTypeServices _typeSvc = new();
        #endregion

        #region Observable Properties
        [ObservableProperty] private ObservableCollection<DishType> dishTypes = new();
        [ObservableProperty] private DishType? selectedDishType;
        private ObservableCollection<Dish> allDishes = new();
        [ObservableProperty] private ObservableCollection<Dish> availableDishes = new();
        #endregion

        #region Constructor
        public OrderViewModel()
        {
            LoadTypesAndDishes();
        }
        #endregion

        private void LoadTypesAndDishes()
        {
            // 1) load types (you can prepend an “All” pseudo-type if you want)
            var types = _typeSvc.GetAll(); // List<DishType>
            DishTypes = new ObservableCollection<DishType>(types);

            // 2) load all dishes once (cache)
            var dishes = _dishSvc.GetAll(); // List<Dish>
            allDishes = new ObservableCollection<Dish>(dishes);

            // 3) initial view = all
            AvailableDishes = new ObservableCollection<Dish>(allDishes);
        }

        [RelayCommand]
        private void ApplyDishTypeFilter(DishType? type)
        {
            if (type is null)
            {
                // show all
                AvailableDishes = new ObservableCollection<Dish>(allDishes);
                return;
            }

            var filtered = allDishes.Where(d => d.Dish_Type?.DishType_ID == type.DishType_ID);
            AvailableDishes = new ObservableCollection<Dish>(filtered);
        }

        //[RelayCommand]
        //private void PrintBill()
        //{
        //    // 1) Create/pull the finalized bill (already saved to DB ideally)
        //    // var bill = _billing.CreateBill(CurrentOrder, SelectedDiscount?.Discount_ID);

        //    // If you already have a computed bill variable, use that instead:
        //    var bill = CurrentBill; // <-- your existing bill object

        //    // 2) Configure (once; you can move this to App settings later)
        //    var options = new BillPrinterOptions
        //    {
        //        PrinterName = null,          // default printer
        //        MaxCharsPerLine = 42,        // 58mm: 42–48; 80mm: 56–64
        //        VatPercent = (double)bill.VatPercent, // use snapshot from bill
        //        RestaurantName = "RestND",
        //        AddressLine = "רח' הדוגמה 123, חיפה",
        //        PhoneLine = "04-1234567",
        //        FontFamily = "Arial Unicode MS",
        //        FooterLine = "תודה רבה ולהתראות!"
        //    };

        //    // 3) Print
        //    new BillPrinterService(CurrentOrder, bill, options).Print();
        //}


    }
}
