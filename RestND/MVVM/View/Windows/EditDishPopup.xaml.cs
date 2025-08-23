using RestND.MVVM.Model;
using RestND.MVVM.ViewModel;
using RestND.MVVM.ViewModel.Dishes;
using System;
using System.Windows;
using System.Windows.Data;

namespace RestND.MVVM.View.Windows
{
    public partial class EditDishPopup : Window
    {
        public EditDishPopup(Dish dishToEdit)
        {
            InitializeComponent();
            DataContext = new EditDishViewModel(dishToEdit);

            // Hook both search bar by product name
            //ProductSearch.SearchTextChanged += (s, text) => ApplyProductFilter(text);
            // Initial filters after the view is ready
            //Loaded += (_, __) =>
            //{
            //    ApplyProductFilter(ProductSearch.SearchText ?? string.Empty);
            //};
        }

        //Search bar Filter: ProductSelections by Product_Name
        //private void ApplyProductFilter(string searchText)
        //{
        //    var vm = DataContext as DishViewModel;
        //    var items = vm?.ProductSelections;   // ObservableCollection<SelectableProduct>
        //    if (items is null) return;

        //    var view = CollectionViewSource.GetDefaultView(items);
        //    if (view is null) return;

        //    var q = (searchText ?? string.Empty).Trim();
        //    if (string.IsNullOrWhiteSpace(q))
        //    {
        //        view.Filter = null;
        //        view.Refresh();
        //        return;
        //    }

        //    view.Filter = o =>
        //    {
        //        var sp = o as SelectableProduct;
        //        return !string.IsNullOrEmpty(sp?.Product_Name)
        //            && sp.Product_Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
        //    };
        //    view.Refresh();
        //}
    }
}
