using DocumentFormat.OpenXml.Drawing.Charts;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.View.Windows;
using RestND.MVVM.ViewModel;
using RestND.MVVM.ViewModel.Dishes;
using RestND.Validations;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace RestND.MVVM.View
{
    public partial class DishWindow : Window
    {
        private readonly DishValidator _dishValidator = new();

        #region Constructor
        public DishWindow()
        {
            InitializeComponent();
            this.DataContext = new DishViewModel();

            // Hook both search bars
            DishSearch.SearchTextChanged += (s, text) => ApplyDishFilter(text);
            ProductSearch.SearchTextChanged += (s, text) => ApplyProductFilter(text);

            // Initial filters after the view is ready
            Loaded += (_, __) =>
            {
                ApplyDishFilter(DishSearch.SearchText ?? string.Empty);
                ApplyProductFilter(ProductSearch.SearchText ?? string.Empty);
            };
        }
        #endregion

        #region Minimize, Maximize, Close + Drag window Methods
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Opening Update Dish Popup Method
        private void UpdateDishBtn_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DishViewModel;
            if (_dishValidator.CheckIfNull(vm.SelectedDish, out string errorMessage))
            {// if a dish was selected for an update
                var editWindow = new EditDishPopup(vm.SelectedDish)
                {
                    Owner = this,
                    DataContext = new EditDishViewModel(vm.SelectedDish)
                };

                this.Opacity = 0.4;
                editWindow.Owner = this;
                editWindow.ShowDialog();
                this.Opacity = 1.0;

                vm.LoadDishesCommand.Execute(null);
            }
            else
            {
                vm.DishErrorMessage = errorMessage;
                return;
            }
        }
        #endregion

        #region method for collapsing back the products details row on the second click.
        private void Row_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not DataGridRow row) return;

            // if click happened inside the details area, ignore (don’t collapse)
            if (FindAncestor<DataGridDetailsPresenter>(e.OriginalSource as DependencyObject) != null)
                return;

            // second click on the same row -> collapse by deselecting
            if (row.IsSelected)
            {
                row.IsSelected = false;   // this collapses details (VisibleWhenSelected)
                e.Handled = true;         // stop the default re-select
            }
        }
        // helper method to Row_PreviewMouseLeftButtonDown().
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null && current is not T)
                current = VisualTreeHelper.GetParent(current);
            return current as T;
        }
        #endregion

        #region Filtering Methods
        //Search bar Filter 1: Dishes by Dish_Name
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

        //Search bar Filter 2: ProductSelections by Product_Name
        private void ApplyProductFilter(string searchText)
        {
            var vm = DataContext as DishViewModel;
            var items = vm?.ProductSelections;   // ObservableCollection<SelectableProduct>
            if (items is null) return;

            var view = CollectionViewSource.GetDefaultView(items);
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
                var sp = o as SelectableProduct;
                return !string.IsNullOrEmpty(sp?.Product_Name)
                    && sp.Product_Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
            };
            view.Refresh();
        }
        #endregion

    }
}
