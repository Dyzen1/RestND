using RestND.MVVM.Model.Dishes;
using RestND.MVVM.ViewModel;
using RestND.MVVM.ViewModel.Dishes;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace RestND.MVVM.View.Windows
{
    public partial class SoftDrinkWindow : Window
    {
        public SoftDrinkWindow()
        {
            InitializeComponent();
            this.DataContext = new SoftDrinkViewModel();
            Loaded += (_, __) => NameBox?.Focus();
            // Hook to search bar
            DrinkSearch.SearchTextChanged += (s, text) => ApplyDishFilter(text);
            // Initial filters after the view is ready
            Loaded += (_, __) =>
            {
                ApplyDishFilter(DrinkSearch.SearchText ?? string.Empty);
            };
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        #region Search bar filter method
        //Search bar Filter: Drinks by Drink_Name
        private void ApplyDishFilter(string searchText)
        {
            var vm = DataContext as SoftDrinkViewModel;
            var items = vm?.SoftDrinks;
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
                var d = o as SoftDrink;
                return !string.IsNullOrEmpty(d?.Drink_Name)
                    && d.Drink_Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
            };
            view.Refresh();
        }
        #endregion
    }
}
