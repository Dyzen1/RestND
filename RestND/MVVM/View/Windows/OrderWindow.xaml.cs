using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using RestND.MVVM.ViewModel;
using RestND.MVVM.ViewModel.Orders;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RestND.MVVM.View.Windows
{
    public partial class OrderWindow : Window
    {

        private Point _dragStart;

        #region Constructors
        public OrderWindow()
        {
            InitializeComponent();
            this.DataContext = new OrderViewModel();
        }

        public OrderWindow(Order order)
        { 
            InitializeComponent();
            this.DataContext = new OrderViewModel(order);
            // Search bar event hook
            DishSearch.SearchTextChanged += (s, text) => ApplyDishFilter(text);
            // Initial filters
            Loaded += (_, __) =>
            {
                ApplyDishFilter(DishSearch.SearchText ?? string.Empty);
            };
        }
        #endregion

        #region Dishes Filter Method For Search Bar
        private void ApplyDishFilter(string searchText)
        {
            var vm = DataContext as DishViewModel;
            var items = vm?.Dishes;
            if (items is null) return;

            var view = CollectionViewSource.GetDefaultView(items);
            if (view is null) return;

            var q = (searchText ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(q))
            {
                view.Filter = null;       // show all
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
        #endregion

        #region Drag and Drop Handlers
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