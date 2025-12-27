using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using RestND.MVVM.ViewModel.Orders;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RestND.MVVM.View.Windows
{
    public partial class OrderWindow : Window
    {
        private Point _dragStart;

        public OrderWindow()
        {
            InitializeComponent();

            var vm = new OrderViewModel();
            DataContext = vm;

            HookSearch();
        }

        public OrderWindow(Order order)
        {
            InitializeComponent();

            var vm = new OrderViewModel(order);

            // ✅ Load ongoing order lines from DB (so you can come back to the order)
            if (order != null && order.Order_ID > 0)
            {
                var lines = new DishInOrderServices().GetByOrderId(order.Order_ID);

                // Replace with the saved lines
                vm.CurrentOrder.DishInOrder = new ObservableCollection<DishInOrder>(lines);

                // keep total correct
                vm.Reload(); // refresh dishes/stock availability
            }

            DataContext = vm;

            HookSearch();
        }

        private void HookSearch()
        {
            DishSearch.SearchTextChanged += (s, text) => ApplyDishFilter(text);

            Loaded += (_, __) =>
            {
                ApplyDishFilter(DishSearch.SearchText ?? string.Empty);
            };
        }

        // ✅ Filter the correct list: AvailableDishes (not DishViewModel)
        private void ApplyDishFilter(string searchText)
        {
            if (DataContext is not OrderViewModel vm) return;

            var view = CollectionViewSource.GetDefaultView(vm.AvailableDishes);
            if (view is null) return;

            var q = (searchText ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(q))
            {
                view.Filter = null;
                view.Refresh();
                return;
            }

            view.Filter = o =>
            {
                var d = o as Dish;
                return !string.IsNullOrEmpty(d?.Dish_Name)
                    && d.Dish_Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
            };

            view.Refresh();
        }

        #region Drag and Drop
        private void DishList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStart = e.GetPosition(null);
        }

        private void DishList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var pos = e.GetPosition(null);
            if (SystemParameters.MinimumHorizontalDragDistance <= Math.Abs(pos.X - _dragStart.X) ||
                SystemParameters.MinimumVerticalDragDistance <= Math.Abs(pos.Y - _dragStart.Y))
            {
                if (sender is ListView lv && lv.SelectedItem is Dish dish)
                {
                    DragDrop.DoDragDrop(lv, dish, DragDropEffects.Copy);
                }
            }
        }

        private void OrderList_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(typeof(Dish)) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private void OrderList_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(Dish))) return;

            var dish = (Dish)e.Data.GetData(typeof(Dish));
            if (DataContext is OrderViewModel vm && vm.AddDishToOrderCommand.CanExecute(dish))
                vm.AddDishToOrderCommand.Execute(dish);
        }
        #endregion
    }
}
