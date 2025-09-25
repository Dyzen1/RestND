using RestND.MVVM.ViewModel.Dishes;
using System.Windows;

namespace RestND.MVVM.View.Windows
{
    public partial class SoftDrinkWindow : Window
    {
        public SoftDrinkWindow()
        {
            InitializeComponent();
            this.DataContext = new SoftDrinkViewModel();
            Loaded += (_, __) => NameBox?.Focus();
        }
    }
}
