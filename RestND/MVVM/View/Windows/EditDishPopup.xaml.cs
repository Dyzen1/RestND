using RestND.MVVM.Model;
using RestND.MVVM.ViewModel;
using System.Windows;

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
