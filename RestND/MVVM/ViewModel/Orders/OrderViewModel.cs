using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using RestND.MVVM.Model.Orders;
using RestND.Data;
using RestND.MVVM.Model;
using System.Collections.Generic;

namespace RestND.MVVM.ViewModel.Orders
{
    public partial class OrderViewModel : ObservableObject
    {
        #region Services
        private readonly OrderServices _orderService;
        private readonly DishInOrderServices _dishInOrderServices;
        private readonly DishServices _dishServices;
        #endregion

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
