using RestND.MVVM.Model;
using RestND.MVVM.View.Windows;
using RestND.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace RestND.MVVM.View
{
    public partial class ProductWindow : Window
    {
        public ProductWindow()
        {
            InitializeComponent();
            this.DataContext = new ProductViewModel();
            // Hook product search bar
            ProductSearch.SearchTextChanged += (s, text) => ApplyProductFilter(text);
            // Initial filter after the view is ready
            Loaded += (_, __) =>
            {
                ApplyProductFilter(ProductSearch.SearchText ?? string.Empty);
            };
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        //close + minimize + maximize window.
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

        //Search bar Filter: ProductSelections by Product_Name
        private void ApplyProductFilter(string searchText)
        {
            var vm = DataContext as ProductViewModel;
            var items = vm?.Products;   // ObservableCollection<SelectableProduct>
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
                var i = o as Inventory;
                return !string.IsNullOrEmpty(i?.Product_Name)
                    && i.Product_Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
            };
            view.Refresh();
        }
    }
}