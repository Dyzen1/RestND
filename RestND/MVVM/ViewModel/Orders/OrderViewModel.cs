using System;
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
            private readonly OrderServices _orderSvc = new();
            private readonly DishServices _dishSvc = new();
            private readonly DishTypeServices _dishTypeSvc = new();
            private readonly DishInOrderServices _dishInOrderSvc = new();

            // ✅ Needed for stock/availability
            private readonly ProductService _productService = new();
            private readonly ProductInDishService _productInDishService = new();
            #endregion

        #region Properties
            [ObservableProperty] private ObservableCollection<DishType> dishTypes = new();
            [ObservableProperty] private ObservableCollection<Dish> allDishes = new();

            // This is what your OrderWindow binds to (ItemSource="{Binding AvailableDishes}")
            [ObservableProperty] private ObservableCollection<Dish> availableDishes = new();

            // ✅ Current inventory snapshot for availability calc
            [ObservableProperty] private ObservableCollection<Inventory> availableProducts = new();

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
                // prime inventory snapshot first (we need it for availability)
                AvailableProducts = new ObservableCollection<Inventory>(_productService.GetAll());

                LoadTypesAndDishes();
                ApplyFilter();          // populates AvailableDishes (by type)
                UpdateDishAvailability(); // compute In_Stock flags

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
            partial void OnSelectedDishTypeChanged(DishType? value)
            {
                ApplyFilter();
                UpdateDishAvailability();
            }

            // When CurrentOrder changes, rewire collection hooks and recompute total
            partial void OnCurrentOrderChanged(Order value)
            {
                UnhookOrderItems();
                HookOrderItems(value);
                RecalculateTotal();
            }
        #endregion

        #region Methods - Dishes, Stock & Filtering
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

        /// <summary>
        /// Re-snapshot inventory and recompute each dish's In_Stock flag.
        /// Call this after any stock-affecting operation.
        /// </summary>
        private void ReloadStockAndAvailability()
        {
            AvailableProducts = new ObservableCollection<Inventory>(_productService.GetAll());
            UpdateDishAvailability();
        }

        /// <summary>
        /// Local availability calc (OrderWindow-only dimming).
        /// Sets Dish.In_Stock based on products_in_dish vs. products.Quantity_Available.
        /// </summary>
        private void UpdateDishAvailability()
        {
            var stockById = AvailableProducts.ToDictionary(p => p.Product_ID, p => p.Quantity_Available);

            foreach (var dish in AvailableDishes)
            {
                // Ensure link rows present
                var usage = dish.ProductUsage;
                if (usage == null || usage.Count == 0)
                {
                    usage = _productInDishService.GetProductsInDish(dish.Dish_ID) ?? new List<ProductInDish>();
                    // fill names if missing (nice-to-have)
                    foreach (var r in usage)
                    {
                        if (string.IsNullOrWhiteSpace(r.Product_Name))
                        {
                            r.Product_Name = AvailableProducts
                                .FirstOrDefault(p => p.Product_ID == r.Product_ID)?.Product_Name ?? r.Product_ID;
                        }
                    }
                    dish.ProductUsage = usage;
                }

                dish.In_Stock = usage.All(link =>
                    stockById.TryGetValue(link.Product_ID, out var have) && have >= link.Amount_Usage
                );
            }
        }

        public void Reload()
        {
            AvailableProducts = new ObservableCollection<Inventory>(_productService.GetAll());
            LoadTypesAndDishes();
            ApplyFilter();
            UpdateDishAvailability();
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

            // Block if not in stock for the Order window flow
            if (!dish.In_Stock)
            {
                MessageBox.Show("This dish is unavailable (insufficient ingredients).",
                                "Unavailable", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // If the item doesn't exist yet in the order — add it
            var existing = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == dish.Dish_ID);
            if (existing == null)
            {
                var newItem = new DishInOrder(dish);
                CurrentOrder.DishInOrder.Add(newItem);
                _dishInOrderSvc.AddDishToOrder(CurrentOrder.Order_ID, newItem);
            }
            else
            {
                // optional: increment if already exists
                existing.Quantity += 1;
                existing.TotalDishPrice += dish.Dish_Price;
                _dishInOrderSvc.UpdateDishInOrder(CurrentOrder.Order_ID, existing);
            }

            // 🧮 Deduct the used ingredients from stock
            _orderSvc.DeductProductQuantities(dish.Dish_ID);     


            // ♻️ Refresh product list and dish availability (for dimming)
            ReloadStockAndAvailability();

            RecalculateTotal();
        }

        [RelayCommand]
        private void IncrementLine(DishInOrder item)
        {
            if (item == null) return;

            var line = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == item.dish.Dish_ID);
            if (line == null) return;

            // Optional: check stock before incrementing (not shown)
            line.Quantity += 1;
            line.TotalDishPrice += item.dish.Dish_Price;
            _dishInOrderSvc.UpdateDishInOrder(CurrentOrder.Order_ID, line);

            RecalculateTotal();
        }

        [RelayCommand]
        private void DecrementLine(DishInOrder item)
        {
            if (item == null) return;

            var line = CurrentOrder.DishInOrder.FirstOrDefault(l => l.dish.Dish_ID == item.dish.Dish_ID);
            if (line == null) return;

            if (line.Quantity > 1)
            {
                line.Quantity -= 1;
                line.TotalDishPrice -= item.dish.Dish_Price;
                _dishInOrderSvc.UpdateDishInOrder(CurrentOrder.Order_ID, line);
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

        [RelayCommand]
        private void PrintBill()
        {
            try
            {
                if (CurrentOrder == null || CurrentOrder.DishInOrder == null || CurrentOrder.DishInOrder.Count == 0)
                {
                    MessageBox.Show("No items in the order to print.", "Print Bill",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // If you already created a Bill object elsewhere, pass it. Otherwise, pass null and let the printer
                // compute totals from the order (or compute a Bill here).
                var printer = new BillPrinter(CurrentOrder, CurrentBill);

                printer.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Printing failed: {ex.Message}", "Print Bill",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}
