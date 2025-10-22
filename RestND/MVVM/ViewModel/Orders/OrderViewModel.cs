using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RestND.MVVM.ViewModel.Orders
{
    public partial class OrderViewModel : ObservableObject
    {
        #region Services
        private readonly DishServices _dishSvc = new();
        private readonly DishTypeServices _dishTypeSvc = new();
        private readonly DishInOrderServices _dishInOrderSvc = new();
        private readonly ProductService _productSvc = new();
        private readonly ProductInDishService _productInDishSvc = new();
        #endregion

        #region Properties
        [ObservableProperty] private ObservableCollection<DishType> dishTypes = new();
        [ObservableProperty] private ObservableCollection<Dish> allDishes = new();
        [ObservableProperty] private ObservableCollection<Dish> availableDishes = new();
        [ObservableProperty] private Order currentOrder = new();
        [ObservableProperty] private Bill currentBill = new();
        [ObservableProperty] private DishType? selectedDishType;
        #endregion

        #region Constructor
        public OrderViewModel()
        {
            LoadTypesAndDishes();
            ApplyFilter();
        }

        public OrderViewModel(Order order) : this()
        {
            CurrentOrder = order;
        }
        #endregion

        #region On Change
        partial void OnSelectedDishTypeChanged(DishType? value)
        {
            ApplyFilter();
        }
        #endregion

        #region Methods
        private void LoadTypesAndDishes()
        {
            var types = _dishTypeSvc.GetAll(); 
            var dishes = _dishSvc.GetAll();
            var inventory = _productSvc.GetAll();

            ApplyAvailabilityState(dishes, inventory);

            DishTypes = new ObservableCollection<DishType>(types);
            AllDishes = new ObservableCollection<Dish>(dishes);
        }

        private void ApplyFilter()
        {
            if (SelectedDishType is null)
            {
                ResetAvailable(AllDishes);
                return;
            }

            var typeName = SelectedDishType.DishType_Name;
            var filtered = AllDishes.Where(d => d.Dish_Type?.DishType_Name == typeName);
            ResetAvailable(filtered);
        }

        // Replace contents in-place so the DataGrid updates reliably
        private void ResetAvailable(IEnumerable<Dish> source)
        {
            AvailableDishes.Clear();
            foreach (var d in source)
                AvailableDishes.Add(d);
        }

        public void Reload()
        {
            LoadTypesAndDishes();
            ApplyFilter();
        }

        [RelayCommand]
        private void AddDishToOrder(Dish dish)
        {
            if (dish is null || !dish.In_Stock) return;
            // checking the item doesnt exists yet in the order.
            var item = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == dish.Dish_ID);
            if (item == null)
                CurrentOrder.DishInOrder.Add(new DishInOrder(dish));
            _dishInOrderSvc.AddDishToOrder(CurrentOrder.Order_ID, new DishInOrder(dish));
        }
        [RelayCommand]
        private void IncrementLine(DishInOrder item)
        {
            if (item == null) return;
            
            var dishInOrder = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == item.dish.Dish_ID);
            dishInOrder.Quantity += 1;
            dishInOrder.TotalDishPrice += item.dish.Dish_Price;
            _dishInOrderSvc.UpdateDishInOrder(CurrentOrder.Order_ID, dishInOrder);
        }

        [RelayCommand]
        private void DecrementLine(DishInOrder item)
        {
            if (item == null) return;

            var dishInOrder = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == item.dish.Dish_ID);
            if (dishInOrder.Quantity > 1)
            {
                dishInOrder.Quantity -= 1;
                dishInOrder.TotalDishPrice -= item.dish.Dish_Price;
                _dishInOrderSvc.UpdateDishInOrder(CurrentOrder.Order_ID, dishInOrder);
            }
            else
            {
                CurrentOrder.DishInOrder.Remove(item);
                _dishInOrderSvc.DeleteDishFromOrder(item.dish.Dish_ID, CurrentOrder.Order_ID);
            }
        }

        [RelayCommand]
        private void RemoveLine(DishInOrder item)
        {
            if (item == null) return;
            CurrentOrder.DishInOrder.Remove(item);
            _dishInOrderSvc.DeleteDishFromOrder(item.dish.Dish_ID, CurrentOrder.Order_ID);
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
        //        RestaurantName = "RestND",
        //        AddressLine = "רח' הדוגמה 123, חיפה",
        //        PhoneLine = "04-1234567",
        //        FontFamily = "Arial Unicode MS",
        //        FooterLine = "תודה רבה ולהתראות!"
        //    };

        //    // 3) Print
        //    new BillPrinterService(CurrentOrder, bill, options).Print();
        //}
        #endregion

        #region Helpers
        private void ApplyAvailabilityState(IEnumerable<Dish> dishes, IEnumerable<Inventory> inventory)
        {
            var stockById = (inventory ?? Enumerable.Empty<Inventory>())
                .GroupBy(p => p.Product_ID)
                .ToDictionary(g => g.Key, g => g.First(), System.StringComparer.OrdinalIgnoreCase);

            foreach (var dish in dishes)
            {
                var usage = _productInDishSvc.GetProductsInDish(dish.Dish_ID) ?? new List<ProductInDish>();
                dish.ProductUsage = usage;
                dish.In_Stock = CanPrepareDish(usage, stockById);
            }
        }

        private static bool CanPrepareDish(IEnumerable<ProductInDish> usage, IReadOnlyDictionary<string, Inventory> stockById)
        {
            if (usage == null)
                return true;

            foreach (var ingredient in usage.Where(u => u?.Is_Active != false))
            {
                if (string.IsNullOrWhiteSpace(ingredient.Product_ID))
                    return false;

                if (!stockById.TryGetValue(ingredient.Product_ID, out var product))
                    return false;

                var remaining = product.Quantity_Available - ingredient.Amount_Usage;
                if (remaining < 0)
                    return false;
            }

            return true;
        }
        #endregion
    }
}
