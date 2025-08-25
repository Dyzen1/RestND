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
        }
    }
}
