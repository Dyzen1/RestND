using RestND.MVVM.Model;
using RestND.MVVM.Model.Employees;
using RestND.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RestND.MVVM.View.Windows
{
    public partial class DishTypeWindow : Window
    {
        public DishTypeWindow()
        {
            InitializeComponent();
            this.DataContext = new DishTypeViewModel();
            // Hook dish types search bar
            DishTypeSearch.SearchTextChanged += (s, text) => ApplyProductFilter(text);
            // Initial filter after the view is ready
            Loaded += (_, __) =>
            {
                ApplyProductFilter(DishTypeSearch.SearchText ?? string.Empty);
            };
        }

        //Search bar Filter: dish types selections by Role_Name
        private void ApplyProductFilter(string searchText)
        {
            var vm = DataContext as DishTypeViewModel;
            var items = vm?.DishTypes;   // ObservableCollection<SelectableProduct>
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
                var i = o as DishType;
                return !string.IsNullOrEmpty(i?.DishType_Name)
                    && i.DishType_Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
            };
            view.Refresh();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
