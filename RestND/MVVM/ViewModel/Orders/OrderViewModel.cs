using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Collections.Specialized;
using System.ComponentModel;

namespace RestND.MVVM.ViewModel.Orders
{
    public partial class OrderViewModel : ObservableObject
    {
        #region Services
        private readonly DishServices _dishSvc = new();
        private readonly DishTypeServices _dishTypeSvc = new();
        private readonly DishInOrderServices _dishInOrderSvc = new();
        #endregion

        #region Properties
        [ObservableProperty] private ObservableCollection<DishType> dishTypes = new();
        [ObservableProperty] private ObservableCollection<Dish> allDishes = new();
        [ObservableProperty] private ObservableCollection<Dish> availableDishes = new();

        // NOTE: we subscribe to CurrentOrder.DishInOrder changes to keep TotalPrice live.
        [ObservableProperty] private Order currentOrder = new();

        [ObservableProperty] private Bill currentBill = new();
        [ObservableProperty] private DishType? selectedDishType;

        // NEW: live total (header)
        [ObservableProperty] private double totalPrice;
        #endregion

        #region Constructor
        public OrderViewModel()
        {
            LoadTypesAndDishes();
            ApplyFilter();

            // ensure we’re hooked to the default order as well
            HookOrderItems(CurrentOrder);
            RecalculateTotal();
        }

        public OrderViewModel(Order order) : this()
        {
            CurrentOrder = order; // triggers OnCurrentOrderChanged -> rewire + recalc
        }
        #endregion

        #region Change Hooks
        // When user selects a type, refresh the list
        partial void OnSelectedDishTypeChanged(DishType? value) => ApplyFilter();

        // When CurrentOrder changes, rewire collection hooks and recompute total
        partial void OnCurrentOrderChanged(Order value)
        {
            UnhookOrderItems();
            HookOrderItems(value);
            RecalculateTotal();
        }
        #endregion

        #region Methods - Dishes & Filtering
        private void LoadTypesAndDishes()
        {
            var types = _dishTypeSvc.GetAll();
            var dishes = _dishSvc.GetAll();

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

        // Replace contents in-place so the ItemsControl updates reliably
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
        #endregion

        #region Methods - Order Live Total
        private void HookOrderItems(Order? order)
        {
            if (order?.DishInOrder is ObservableCollection<DishInOrder> col)
            {
                col.CollectionChanged += Items_CollectionChanged;

                // If DishInOrder implements INotifyPropertyChanged, hook each line
                foreach (var it in col)
                    HookItem(it);
            }
        }

        private void UnhookOrderItems()
        {
            if (CurrentOrder?.DishInOrder is ObservableCollection<DishInOrder> col)
            {
                col.CollectionChanged -= Items_CollectionChanged;
                foreach (var it in col)
                    UnhookItem(it);
            }
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (DishInOrder it in e.NewItems)
                    HookItem(it);
            }
            if (e.OldItems != null)
            {
                foreach (DishInOrder it in e.OldItems)
                    UnhookItem(it);
            }
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
            // Common names are "Quantity", "TotalDishPrice" or "dish" (if price could change)
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

            // Prefer TotalDishPrice if you maintain it; otherwise fall back to Dish_Price * Quantity
            double sum = CurrentOrder.DishInOrder.Sum(x =>
            {
                if (x.TotalDishPrice > 0)
                    return (double)x.TotalDishPrice;
                return (double)(x.dish?.Dish_Price ?? 0) * x.Quantity;
            });

            TotalPrice = sum;
        }
        #endregion

        #region Commands
        [RelayCommand]
        private void AddDishToOrder(Dish dish)
        {
            if (dish is null) return;

            // checking the item doesnt exists yet in the order.
            var item = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == dish.Dish_ID);
            if (item == null)
            {
                var newItem = new DishInOrder(dish);
                CurrentOrder.DishInOrder.Add(newItem);
                _dishInOrderSvc.AddDishToOrder(CurrentOrder.Order_ID, newItem);
            }
            else
            {
                // optional: if already exists, increment instead of ignoring
                item.Quantity += 1;
                item.TotalDishPrice += dish.Dish_Price;
                _dishInOrderSvc.UpdateDishInOrder(CurrentOrder.Order_ID, item);
            }

            RecalculateTotal();
        }

        [RelayCommand]
        private void IncrementLine(DishInOrder item)
        {
            if (item == null) return;

            var dishInOrder = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == item.dish.Dish_ID);
            if (dishInOrder == null) return;

            dishInOrder.Quantity += 1;
            dishInOrder.TotalDishPrice += item.dish.Dish_Price;
            _dishInOrderSvc.UpdateDishInOrder(CurrentOrder.Order_ID, dishInOrder);

            RecalculateTotal();
        }

        [RelayCommand]
        private void DecrementLine(DishInOrder item)
        {
            if (item == null) return;

            var dishInOrder = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == item.dish.Dish_ID);
            if (dishInOrder == null) return;

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

            RecalculateTotal();
        }

        [RelayCommand]
        private void RemoveLine(DishInOrder item)
        {
            if (item == null) return;

            CurrentOrder.DishInOrder.Remove(item);
            _dishInOrderSvc.DeleteDishFromOrder(item.dish.Dish_ID, CurrentOrder.Order_ID);

            RecalculateTotal();
        }
        #endregion
    }
}
