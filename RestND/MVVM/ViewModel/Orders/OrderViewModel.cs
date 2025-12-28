using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestND.MVVM.ViewModel.Orders
{
    public partial class OrderViewModel : ObservableObject
    {
        #region Services
        private readonly TableServices _tableSvc = new();
        private readonly HubConnection _tableHub = App.TableHub;
        private readonly HubConnection _inventoryHub = App.InventoryHub;
        private readonly OrderServices _orderSvc = new();
        private readonly DishServices _dishSvc = new();
        private readonly DishTypeServices _dishTypeSvc = new();
        private readonly DishInOrderServices _dishInOrderSvc = new();
        private readonly ProductService _productService = new();
        private readonly ProductInDishService _productInDishService = new();
        #endregion

        #region Events
        public event Action? RequestClose;
        #endregion

        #region Properties
        [ObservableProperty] private ObservableCollection<DishType> dishTypes = new();
        [ObservableProperty] private ObservableCollection<Dish> allDishes = new();
        [ObservableProperty] private ObservableCollection<Order> allOrders = new(); //for history
        [ObservableProperty] private ObservableCollection<Dish> availableDishes = new();

        [ObservableProperty] private ObservableCollection<Inventory> availableProducts = new();

        [ObservableProperty] private Order currentOrder = new();
        [ObservableProperty] private Bill currentBill = new();
        [ObservableProperty] private DishType? selectedDishType;
        [ObservableProperty] private double totalPrice;
        #endregion

        #region Constructor
        public OrderViewModel()
        {
            AvailableProducts = new ObservableCollection<Inventory>(_productService.GetAll());

            LoadTypesAndDishes(); //and orders (for history)
            ApplyFilter();
            UpdateDishAvailability();

            HookOrderItems(CurrentOrder);
            RecalculateTotal();
        }

        public OrderViewModel(Order order) : this()
        {
            CurrentOrder = order;
        }
        #endregion

        #region Change Hooks
        partial void OnSelectedDishTypeChanged(DishType? value)
        {
            ApplyFilter();
            UpdateDishAvailability();
        }

        partial void OnCurrentOrderChanged(Order value)
        {
            UnhookOrderItems();
            HookOrderItems(value);
            RecalculateTotal();
        }
        #endregion

        #region Load / Filter / Availability
        private void LoadTypesAndDishes()
        {
            DishTypes = new ObservableCollection<DishType>(_dishTypeSvc.GetAll());
            AllDishes = new ObservableCollection<Dish>(_dishSvc.GetAll());
            //AllOrders = new ObservableCollection<Order>(_orderSvc.GetAll());
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

        private void ResetAvailable(IEnumerable<Dish> source)
        {
            AvailableDishes.Clear();
            foreach (var d in source)
                AvailableDishes.Add(d);
        }

        private void ReloadStockAndAvailability()
        {
            AvailableProducts = new ObservableCollection<Inventory>(_productService.GetAll());
            UpdateDishAvailability();
        }

        private void UpdateDishAvailability()
        {
            var stockById = AvailableProducts.ToDictionary(p => p.Product_ID, p => p.Quantity_Available);

            foreach (var dish in AvailableDishes)
            {
                var usage = dish.ProductUsage;
                if (usage == null || usage.Count == 0)
                {
                    usage = _productInDishService.GetProductsInDish(dish.Dish_ID) ?? new List<ProductInDish>();
                    dish.ProductUsage = usage;
                }

                dish.In_Stock = usage.All(link =>
                    stockById.TryGetValue(link.Product_ID, out var have) && have >= link.Amount_Usage
                );
            }
        }
        #endregion

        #region Live Total
        private void HookOrderItems(Order? order)
        {
            if (order?.DishInOrder is ObservableCollection<DishInOrder> col)
            {
                col.CollectionChanged += Items_CollectionChanged;
                foreach (var it in col) HookItem(it);
            }
        }

        private void UnhookOrderItems()
        {
            if (CurrentOrder?.DishInOrder is ObservableCollection<DishInOrder> col)
            {
                col.CollectionChanged -= Items_CollectionChanged;
                foreach (var it in col) UnhookItem(it);
            }
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (DishInOrder it in e.NewItems) HookItem(it);

            if (e.OldItems != null)
                foreach (DishInOrder it in e.OldItems) UnhookItem(it);

            RecalculateTotal();
        }

        private void HookItem(DishInOrder it)
        {
            if (it is INotifyPropertyChanged npc)
                npc.PropertyChanged += Item_PropertyChanged;
        }

        private void UnhookItem(DishInOrder it)
        {
            if (it is INotifyPropertyChanged npc)
                npc.PropertyChanged -= Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DishInOrder.Quantity) ||
                e.PropertyName == nameof(DishInOrder.TotalDishPrice) ||
                e.PropertyName == "dish")
            {
                RecalculateTotal();
            }
        }

        private void RecalculateTotal()
        {
            if (CurrentOrder?.DishInOrder == null || CurrentOrder.DishInOrder.Count == 0)
            {
                TotalPrice = 0;
                return;
            }

            TotalPrice = CurrentOrder.DishInOrder.Sum(x =>
            {
                if (x.TotalDishPrice > 0)
                    return x.TotalDishPrice;

                return (x.dish?.Dish_Price ?? 0) * x.Quantity;
            });
        }
        #endregion

        #region Helpers (SAVE ORDER + INVENTORY BROADCAST)
        private void EnsureOrderIsSaved()
        {
            if (CurrentOrder == null) return;
            if (CurrentOrder.Order_ID > 0) return;

            if (CurrentOrder.assignedEmployee == null || CurrentOrder.Table == null || CurrentOrder.People_Count <= 0)
                throw new Exception("Order must have Employee, Table and People_Count before saving.");

            CurrentOrder.Is_Active = true;

            int newId = _orderSvc.AddStartingOrder(CurrentOrder);
            CurrentOrder.Order_ID = newId;

            // ✅ make table occupied immediately (TABLE HUB)
            _tableSvc.UpdateTableStatusByNumber(CurrentOrder.Table.Table_Number, true);
            CurrentOrder.Table.Table_Status = true;
            _ = _tableHub.SendAsync("NotifyTableUpdate", CurrentOrder.Table, "update");
        }

        private void BroadcastInventoryForDish(int dishId)
        {
            try
            {
                var links = _productInDishService.GetProductsInDish(dishId);
                if (links == null || links.Count == 0) return;

                var all = _productService.GetAll();

                foreach (var link in links)
                {
                    var updated = all.FirstOrDefault(p => p.Product_ID == link.Product_ID);
                    if (updated != null)
                        _ = _inventoryHub.SendAsync("NotifyInventoryUpdate", updated, "update");
                }
            }
            catch { }
        }
        #endregion

        #region Commands (Add / Inc / Dec / Remove)
        [RelayCommand]
        private void AddDishToOrder(Dish dish)
        {
            if (dish is null) return;

            if (!dish.In_Stock)
            {
                MessageBox.Show("This dish is unavailable (insufficient ingredients).",
                    "Unavailable", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EnsureOrderIsSaved();

            var existing = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == dish.Dish_ID);

            if (existing == null)
            {
                var newItem = new DishInOrder(dish);
                CurrentOrder.DishInOrder.Add(newItem);
                _dishInOrderSvc.AddDishToOrder(CurrentOrder.Order_ID, newItem);
                _orderSvc.AdjustProductQuantities(dish.Dish_ID, +1);
            }
            else
            {
                existing.Quantity += 1;
                existing.TotalDishPrice += dish.Dish_Price;
                _dishInOrderSvc.UpdateDishInOrder(CurrentOrder.Order_ID, existing);
                _orderSvc.AdjustProductQuantities(dish.Dish_ID, +1);
            }

            BroadcastInventoryForDish(dish.Dish_ID);
            ReloadStockAndAvailability();
            RecalculateTotal();
        }

        [RelayCommand]
        private void IncrementLine(DishInOrder item)
        {
            if (item == null) return;

            EnsureOrderIsSaved();

            var line = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == item.dish.Dish_ID);
            if (line == null) return;

            ReloadStockAndAvailability();
            if (!line.dish.In_Stock)
            {
                MessageBox.Show("Insufficient ingredients to add more of this dish.",
                    "Unavailable", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            line.Quantity += 1;
            line.TotalDishPrice += item.dish.Dish_Price;
            _dishInOrderSvc.UpdateDishInOrder(CurrentOrder.Order_ID, line);

            _orderSvc.AdjustProductQuantities(item.dish.Dish_ID, +1);

            BroadcastInventoryForDish(item.dish.Dish_ID);
            ReloadStockAndAvailability();
            RecalculateTotal();
        }

        [RelayCommand]
        private void DecrementLine(DishInOrder item)
        {
            if (item == null) return;

            EnsureOrderIsSaved();

            var line = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == item.dish.Dish_ID);
            if (line == null) return;

            if (line.Quantity > 1)
            {
                line.Quantity -= 1;
                line.TotalDishPrice -= item.dish.Dish_Price;
                _dishInOrderSvc.UpdateDishInOrder(CurrentOrder.Order_ID, line);
                _orderSvc.AdjustProductQuantities(item.dish.Dish_ID, -1);
            }
            else
            {
                CurrentOrder.DishInOrder.Remove(item);
                _dishInOrderSvc.DeleteDishFromOrder(item.dish.Dish_ID, CurrentOrder.Order_ID);
                _orderSvc.AdjustProductQuantities(item.dish.Dish_ID, -1);
            }

            BroadcastInventoryForDish(item.dish.Dish_ID);
            ReloadStockAndAvailability();
            RecalculateTotal();
        }

        [RelayCommand]
        private void RemoveLine(DishInOrder item)
        {
            if (item == null) return;

            EnsureOrderIsSaved();

            int qty = item.Quantity;

            CurrentOrder.DishInOrder.Remove(item);
            _dishInOrderSvc.DeleteDishFromOrder(item.dish.Dish_ID, CurrentOrder.Order_ID);

            _orderSvc.AdjustProductQuantities(item.dish.Dish_ID, -qty);

            BroadcastInventoryForDish(item.dish.Dish_ID);
            ReloadStockAndAvailability();
            RecalculateTotal();
        }
        #endregion

        #region Print Commands
        [RelayCommand]
        private async Task PrintTicket()
        {
            EnsureOrderIsSaved();

            if (CurrentOrder == null || CurrentOrder.Table == null) return;

            if (CurrentOrder.DishInOrder == null || CurrentOrder.DishInOrder.Count == 0)
            {
                MessageBox.Show("No items in the order to print.", "Print Ticket",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var printer = new TicketPrinter(CurrentOrder);
            printer.Print();

            // ✅ occupy table (TABLE HUB)
            int tableNum = CurrentOrder.Table.Table_Number;

            bool ok = _tableSvc.UpdateTableStatusByNumber(tableNum, true);
            if (ok)
            {
                CurrentOrder.Table.Table_Status = true;
                await _tableHub.SendAsync("NotifyTableUpdate", CurrentOrder.Table, "update");
            }
        }

        [RelayCommand]
        private async Task PrintBill()
        {
            EnsureOrderIsSaved();

            if (CurrentOrder == null || CurrentOrder.Table == null) return;

            if (CurrentOrder.DishInOrder == null || CurrentOrder.DishInOrder.Count == 0)
            {
                MessageBox.Show("No items in the order to print.", "Print Bill",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            RecalculateTotal();

            CurrentOrder.Bill ??= new Bill();
            CurrentOrder.Bill.Price = TotalPrice;

            var printer = new BillPrinter(CurrentOrder, CurrentBill);
            printer.Print();

            // close order in DB + save total
            _orderSvc.CloseOrder(CurrentOrder.Order_ID, TotalPrice);

            // ✅ FREE THE TABLE (opposite of CreateOrder)
            int tableNum = CurrentOrder.Table.Table_Number;

            bool ok = _tableSvc.UpdateTableStatusByNumber(tableNum, false);
            if (ok)
            {
                CurrentOrder.Table.Table_Status = false;

                
                await _tableHub.SendAsync("NotifyTableUpdate", CurrentOrder.Table, "update");

                
                RequestClose?.Invoke();
            }
        }
        #endregion

        public void Reload()
        {
            AvailableProducts = new ObservableCollection<Inventory>(_productService.GetAll());
            LoadTypesAndDishes();
            ApplyFilter();
            UpdateDishAvailability();
            RecalculateTotal();
        }
    }
}
